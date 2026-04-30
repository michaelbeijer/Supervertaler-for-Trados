#!/usr/bin/env python3
"""
Supervertaler for Trados – Codebase Health Scanner
===================================================
A lightweight static analysis script for AI-assisted C#/.NET codebases.
Flags potential issues arising from multi-session AI code generation:
inconsistency, over-abstraction, duplication, security smells, and more.

Usage:
    python scan_codebase.py /path/to/your/csharp/project

Requirements:
    Python 3.8+ (no external dependencies)
"""

import os
import re
import sys
import json
from pathlib import Path
from collections import Counter, defaultdict
from dataclasses import dataclass, field
from typing import List, Dict, Tuple, Optional


@dataclass
class Finding:
    category: str
    severity: str  # "info", "warning", "error"
    file: str
    line: Optional[int]
    message: str


@dataclass
class ScanResults:
    findings: List[Finding] = field(default_factory=list)
    stats: Dict[str, int] = field(default_factory=dict)

    def add(self, category: str, severity: str, file: str, line: Optional[int], message: str):
        self.findings.append(Finding(category, severity, file, line, message))

    def summary(self) -> Dict[str, int]:
        counts = Counter()
        for f in self.findings:
            counts[f"{f.category} ({f.severity})"] += 1
        return dict(counts.most_common())


def find_cs_files(root: str) -> List[Path]:
    """Find all C# source files, excluding bin/obj/packages directories."""
    excluded = {"bin", "obj", "packages", ".vs", "node_modules", ".git"}
    cs_files = []
    for dirpath, dirnames, filenames in os.walk(root):
        dirnames[:] = [d for d in dirnames if d not in excluded]
        for f in filenames:
            if f.endswith(".cs"):
                cs_files.append(Path(dirpath) / f)
    return cs_files


def read_file(path: Path) -> List[str]:
    """Read file lines, handling encoding issues."""
    try:
        with open(path, "r", encoding="utf-8-sig") as f:
            return f.readlines()
    except UnicodeDecodeError:
        with open(path, "r", encoding="latin-1") as f:
            return f.readlines()


# ---------------------------------------------------------------------------
# Individual scanners
# ---------------------------------------------------------------------------

def scan_async_antipatterns(lines: List[str], filepath: str, results: ScanResults):
    """Detect .Result, .Wait(), and .GetAwaiter().GetResult() blocking calls."""
    for i, line in enumerate(lines, 1):
        stripped = line.strip()
        if stripped.startswith("//") or stripped.startswith("*"):
            continue
        if re.search(r"\.\s*Result\b", line) and "TaskResult" not in line:
            results.add("async-antipattern", "warning", filepath, i,
                        f"Possible blocking .Result call: {stripped[:120]}")
        if re.search(r"\.\s*Wait\s*\(", line):
            results.add("async-antipattern", "warning", filepath, i,
                        f"Possible blocking .Wait() call: {stripped[:120]}")
        if ".GetAwaiter().GetResult()" in line:
            results.add("async-antipattern", "warning", filepath, i,
                        f"Blocking .GetAwaiter().GetResult(): {stripped[:120]}")


def scan_secret_smells(lines: List[str], filepath: str, results: ScanResults):
    """Flag potential hardcoded secrets, API keys, or credentials."""
    patterns = [
        (r'(?i)(api[_-]?key|apikey|secret|password|token|credential)\s*=\s*"[^"]{8,}"',
         "Possible hardcoded secret/API key"),
        (r'(?i)(sk-[a-zA-Z0-9]{20,})',
         "Possible OpenAI API key"),
        (r'(?i)(AIza[a-zA-Z0-9_-]{30,})',
         "Possible Google API key"),
        (r'(?i)(sk-ant-[a-zA-Z0-9_-]{20,})',
         "Possible Anthropic API key"),
        (r'(?i)bearer\s+[a-zA-Z0-9_\-.]{20,}',
         "Possible hardcoded bearer token"),
    ]
    for i, line in enumerate(lines, 1):
        stripped = line.strip()
        if stripped.startswith("//"):
            continue
        for pattern, msg in patterns:
            if re.search(pattern, line):
                results.add("security", "error", filepath, i, f"{msg}: {stripped[:100]}...")
                break


def scan_logging_smells(lines: List[str], filepath: str, results: ScanResults):
    """Detect inconsistent logging and potential data leaks in logs."""
    log_methods = set()
    for i, line in enumerate(lines, 1):
        stripped = line.strip()
        if "Console.Write" in line:
            log_methods.add("Console")
            results.add("logging", "info", filepath, i,
                        "Console.Write used – consider a proper logger for production")
        if "Debug.Write" in line:
            log_methods.add("Debug")
        if re.search(r"(log|logger|Log|Logger)\.(Info|Debug|Warn|Error|Fatal|Trace)", line):
            log_methods.add("Logger")
        # Check for sensitive data being logged
        if re.search(r"(?i)(log|write|trace|debug).*?(password|secret|key|token|credential)", line):
            if not stripped.startswith("//"):
                results.add("security", "warning", filepath, i,
                            f"Possible sensitive data in log output: {stripped[:120]}")
    return log_methods


def scan_httpclient_usage(lines: List[str], filepath: str, results: ScanResults):
    """Detect new HttpClient() instantiation (should be reused/injected)."""
    for i, line in enumerate(lines, 1):
        if "new HttpClient(" in line or "new HttpClient()" in line:
            results.add("performance", "warning", filepath, i,
                        "new HttpClient() – consider reusing a single instance or using IHttpClientFactory")


def scan_method_length(lines: List[str], filepath: str, results: ScanResults):
    """Flag methods that are excessively long."""
    brace_depth = 0
    method_start = None
    method_name = None
    in_method = False

    for i, line in enumerate(lines, 1):
        stripped = line.strip()
        # Rough method detection
        method_match = re.match(
            r'(?:public|private|protected|internal|static|async|override|virtual|sealed|\s)+'
            r'\S+\s+(\w+)\s*[\(<]', stripped)
        if method_match and not stripped.startswith("//") and "{" not in stripped[:stripped.find("(") if "(" in stripped else 0]:
            pass  # declaration without opening brace on same line

        open_braces = stripped.count("{")
        close_braces = stripped.count("}")

        if method_match and not in_method:
            method_name = method_match.group(1)
            method_start = i
            in_method = True
            brace_depth = 0

        brace_depth += open_braces - close_braces

        if in_method and brace_depth <= 0 and method_start and i > method_start:
            length = i - method_start
            if length > 80:
                results.add("maintainability", "warning", filepath, method_start,
                            f"Method '{method_name}' is {length} lines long – consider breaking it up")
            elif length > 50:
                results.add("maintainability", "info", filepath, method_start,
                            f"Method '{method_name}' is {length} lines – getting long")
            in_method = False
            method_name = None
            method_start = None


def scan_single_impl_interfaces(all_files_content: Dict[str, List[str]], results: ScanResults):
    """Find interfaces with only one implementation."""
    interfaces = {}  # name -> file
    implementations = defaultdict(list)  # interface_name -> [(class, file)]

    for filepath, lines in all_files_content.items():
        for i, line in enumerate(lines, 1):
            # Find interface declarations
            iface_match = re.match(r'\s*(?:public|internal)\s+interface\s+(I\w+)', line)
            if iface_match:
                interfaces[iface_match.group(1)] = filepath

            # Find class declarations that implement interfaces
            class_match = re.match(r'\s*(?:public|internal|private)\s+(?:sealed\s+|abstract\s+)?class\s+(\w+)\s*:\s*(.*)', line)
            if class_match:
                class_name = class_match.group(1)
                parents = class_match.group(2)
                for parent in parents.split(","):
                    parent = parent.strip().split("<")[0]  # Remove generic params
                    if parent.startswith("I") and parent[1:2].isupper():
                        implementations[parent].append((class_name, filepath))

    for iface_name, iface_file in interfaces.items():
        impls = implementations.get(iface_name, [])
        if len(impls) == 1:
            results.add("over-abstraction", "info", iface_file, None,
                        f"Interface '{iface_name}' has only one implementation: '{impls[0][0]}' – may be unnecessary abstraction")
        elif len(impls) == 0:
            results.add("dead-code", "warning", iface_file, None,
                        f"Interface '{iface_name}' has no implementations – dead code?")


def scan_todo_markers(lines: List[str], filepath: str, results: ScanResults):
    """Find TODO, FIXME, HACK, XXX markers."""
    for i, line in enumerate(lines, 1):
        match = re.search(r'\b(TODO|FIXME|HACK|XXX|WORKAROUND)\b', line, re.IGNORECASE)
        if match:
            results.add("todo-marker", "info", filepath, i,
                        f"{match.group(1)}: {line.strip()[:120]}")


def scan_magic_numbers(lines: List[str], filepath: str, results: ScanResults):
    """Flag suspicious magic numbers (excluding 0, 1, 2, common timeouts)."""
    common_safe = {0, 1, 2, -1, 10, 100, 1000, 60, 3600, 255, 256, 1024, 2048, 4096}
    for i, line in enumerate(lines, 1):
        stripped = line.strip()
        if stripped.startswith("//") or "const " in line or "enum " in line:
            continue
        # Find standalone numbers in comparisons, assignments, method args
        for match in re.finditer(r'(?<![.\w])(\d{3,})(?!\w)', stripped):
            num = int(match.group(1))
            if num not in common_safe and num < 100000:
                results.add("maintainability", "info", filepath, i,
                            f"Magic number {num}: {stripped[:120]}")


def scan_duplicate_usings(lines: List[str], filepath: str, results: ScanResults):
    """Find duplicate using statements within a file."""
    usings = []
    for i, line in enumerate(lines, 1):
        match = re.match(r'\s*using\s+([\w.]+)\s*;', line)
        if match:
            ns = match.group(1)
            if ns in usings:
                results.add("consistency", "info", filepath, i,
                            f"Duplicate using: {ns}")
            usings.append(ns)


def scan_empty_catch(lines: List[str], filepath: str, results: ScanResults):
    """Flag empty catch blocks (swallowed exceptions)."""
    for i, line in enumerate(lines, 1):
        if re.match(r'\s*catch\s*(\([^)]*\))?\s*\{?\s*\}', line):
            results.add("error-handling", "warning", filepath, i,
                        "Empty catch block – exception swallowed silently")
        # Also check for catch followed by just a closing brace
        if re.match(r'\s*catch\b', line):
            # Look ahead for empty body
            for j in range(i, min(i + 3, len(lines))):
                if lines[j].strip() == "}":
                    # Check if there's any code between catch and }
                    between = "".join(lines[i:j]).strip()
                    if re.match(r'catch\s*(\([^)]*\))?\s*\{\s*$', between):
                        results.add("error-handling", "warning", filepath, i,
                                    "Empty catch block – exception swallowed silently")
                    break


def scan_cert_validation_disabled(lines: List[str], filepath: str, results: ScanResults):
    """Detect disabled SSL certificate validation."""
    for i, line in enumerate(lines, 1):
        if "ServerCertificateCustomValidationCallback" in line:
            results.add("security", "error", filepath, i,
                        "Custom certificate validation callback – verify this isn't disabling SSL checks")
        if "ServicePointManager.ServerCertificateValidationCallback" in line:
            results.add("security", "error", filepath, i,
                        "Global certificate validation override – potential security risk")


def scan_localhost_binding(lines: List[str], filepath: str, results: ScanResults):
    """Check for network binding patterns – important for Okapi sidecar communication."""
    for i, line in enumerate(lines, 1):
        if re.search(r'(0\.0\.0\.0|IPAddress\.Any|\*:\d+|INADDR_ANY)', line):
            results.add("security", "warning", filepath, i,
                        "Binding to all interfaces (0.0.0.0) – should this be localhost only?")
        if "http://localhost" in line or "http://127.0.0.1" in line:
            # Not necessarily bad, but worth flagging for review
            results.add("network", "info", filepath, i,
                        f"Localhost HTTP connection – verify this is intentional: {line.strip()[:100]}")


def collect_stats(all_files: List[Path], all_content: Dict[str, List[str]]) -> Dict[str, int]:
    """Gather basic codebase statistics."""
    total_lines = 0
    total_blank = 0
    total_comment = 0
    total_code = 0

    for filepath, lines in all_content.items():
        for line in lines:
            total_lines += 1
            stripped = line.strip()
            if not stripped:
                total_blank += 1
            elif stripped.startswith("//") or stripped.startswith("/*") or stripped.startswith("*"):
                total_comment += 1
            else:
                total_code += 1

    return {
        "total_cs_files": len(all_files),
        "total_lines": total_lines,
        "code_lines": total_code,
        "comment_lines": total_comment,
        "blank_lines": total_blank,
        "comment_ratio": round(total_comment / max(total_code, 1) * 100, 1),
    }


# ---------------------------------------------------------------------------
# Report generation
# ---------------------------------------------------------------------------

def generate_report(results: ScanResults, stats: Dict, output_path: str):
    """Generate a markdown report."""
    lines = []
    lines.append("# Supervertaler for Trados – Codebase Scan Report\n")
    lines.append(f"Generated by scan_codebase.py\n")

    # Stats
    lines.append("## Codebase statistics\n")
    lines.append(f"| Metric | Value |")
    lines.append(f"|--------|-------|")
    for key, value in stats.items():
        label = key.replace("_", " ").title()
        if key == "comment_ratio":
            lines.append(f"| {label} | {value}% |")
        else:
            lines.append(f"| {label} | {value:,} |")
    lines.append("")

    # Summary
    lines.append("## Findings summary\n")
    summary = results.summary()
    if not summary:
        lines.append("No findings – clean codebase!\n")
    else:
        lines.append(f"| Category | Count |")
        lines.append(f"|----------|-------|")
        for cat, count in summary.items():
            lines.append(f"| {cat} | {count} |")
    lines.append("")

    # Group findings by severity
    errors = [f for f in results.findings if f.severity == "error"]
    warnings = [f for f in results.findings if f.severity == "warning"]
    infos = [f for f in results.findings if f.severity == "info"]

    if errors:
        lines.append("## Errors (fix before release)\n")
        for f in errors:
            loc = f"{f.file}:{f.line}" if f.line else f.file
            lines.append(f"- **[{f.category}]** `{loc}` – {f.message}")
        lines.append("")

    if warnings:
        lines.append("## Warnings (should investigate)\n")
        for f in warnings:
            loc = f"{f.file}:{f.line}" if f.line else f.file
            lines.append(f"- **[{f.category}]** `{loc}` – {f.message}")
        lines.append("")

    if infos:
        lines.append("## Info (review when time permits)\n")
        for f in infos:
            loc = f"{f.file}:{f.line}" if f.line else f.file
            lines.append(f"- **[{f.category}]** `{loc}` – {f.message}")
        lines.append("")

    lines.append("---\n")
    lines.append("## Next steps\n")
    lines.append("1. Address all **errors** first – these are potential security or correctness issues")
    lines.append("2. Review **warnings** – these may indicate performance problems or inconsistencies")
    lines.append("3. Consider **info** items during your next refactoring pass")
    lines.append("4. Run `dotnet list package --vulnerable` separately for NuGet dependency checks")
    lines.append("5. Re-run this scan after major AI-assisted coding sessions\n")

    report_text = "\n".join(lines)
    with open(output_path, "w", encoding="utf-8") as f:
        f.write(report_text)

    return report_text


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main():
    if len(sys.argv) < 2:
        print("Usage: python scan_codebase.py /path/to/csharp/project [--output report.md]")
        print()
        print("Scans a C#/.NET codebase for common issues in AI-generated code:")
        print("  - Async anti-patterns (.Result, .Wait())")
        print("  - Hardcoded secrets and API keys")
        print("  - Inconsistent logging")
        print("  - HttpClient misuse")
        print("  - Long methods")
        print("  - Single-implementation interfaces")
        print("  - TODO/FIXME markers")
        print("  - Magic numbers")
        print("  - Empty catch blocks")
        print("  - SSL certificate validation bypasses")
        print("  - Network binding patterns")
        sys.exit(1)

    root = sys.argv[1]
    output = "scan-report.md"
    if "--output" in sys.argv:
        idx = sys.argv.index("--output")
        if idx + 1 < len(sys.argv):
            output = sys.argv[idx + 1]

    if not os.path.isdir(root):
        print(f"Error: '{root}' is not a directory")
        sys.exit(1)

    print(f"Scanning: {root}")
    cs_files = find_cs_files(root)
    print(f"Found {len(cs_files)} C# files")

    if not cs_files:
        print("No C# files found. Check the path.")
        sys.exit(1)

    # Read all files
    all_content = {}
    for fp in cs_files:
        rel = str(fp.relative_to(root))
        all_content[rel] = read_file(fp)

    results = ScanResults()
    all_log_methods = set()

    # Run per-file scanners
    for filepath, lines in all_content.items():
        scan_async_antipatterns(lines, filepath, results)
        scan_secret_smells(lines, filepath, results)
        log_methods = scan_logging_smells(lines, filepath, results)
        all_log_methods.update(log_methods)
        scan_httpclient_usage(lines, filepath, results)
        scan_method_length(lines, filepath, results)
        scan_todo_markers(lines, filepath, results)
        scan_magic_numbers(lines, filepath, results)
        scan_duplicate_usings(lines, filepath, results)
        scan_empty_catch(lines, filepath, results)
        scan_cert_validation_disabled(lines, filepath, results)
        scan_localhost_binding(lines, filepath, results)

    # Run cross-file scanners
    scan_single_impl_interfaces(all_content, results)

    # Check for mixed logging
    if len(all_log_methods) > 1:
        results.add("consistency", "warning", "(project-wide)", None,
                     f"Mixed logging approaches used: {', '.join(sorted(all_log_methods))} – consider standardising")

    # Gather stats
    stats = collect_stats(cs_files, all_content)

    # Generate report
    report = generate_report(results, stats, output)

    # Print summary to console
    print(f"\n{'='*60}")
    print(f"SCAN COMPLETE")
    print(f"{'='*60}")
    print(f"Files scanned: {stats['total_cs_files']}")
    print(f"Lines of code: {stats['code_lines']:,}")
    print(f"Findings: {len(results.findings)}")
    print(f"  Errors:   {sum(1 for f in results.findings if f.severity == 'error')}")
    print(f"  Warnings: {sum(1 for f in results.findings if f.severity == 'warning')}")
    print(f"  Info:     {sum(1 for f in results.findings if f.severity == 'info')}")
    print(f"\nFull report: {output}")
    print()

    # Print errors to console immediately
    errors = [f for f in results.findings if f.severity == "error"]
    if errors:
        print("ERRORS (fix these first):")
        for f in errors:
            loc = f"{f.file}:{f.line}" if f.line else f.file
            print(f"  [{f.category}] {loc} – {f.message}")


if __name__ == "__main__":
    main()
