using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Supervertaler.Trados.Core;

namespace Supervertaler.Trados.Controls
{
    /// <summary>
    /// Dialog that prompts the user when adding a term whose source or target
    /// already exists in the termbase. Offers to merge as a synonym (quick or
    /// via the term entry editor), keep both as separate entries, or cancel.
    ///
    /// DialogResult.Yes    = Add as Synonym (quick — insert and done)
    /// DialogResult.Retry  = Add as Synonym &amp; open Term Entry Editor for review
    /// DialogResult.No     = Keep Both (create a separate entry)
    /// DialogResult.Cancel = Cancel (abort the add operation)
    /// </summary>
    public class MergePromptDialog : Form
    {
        private readonly List<MergeMatch> _matches;
        private readonly string _newSource;
        private readonly string _newTarget;

        /// <summary>
        /// The list of merge matches the user is responding to.
        /// </summary>
        public List<MergeMatch> Matches => _matches;

        public MergePromptDialog(
            List<MergeMatch> matches, string newSource, string newTarget)
        {
            _matches = matches ?? new List<MergeMatch>();
            _newSource = newSource ?? "";
            _newTarget = newTarget ?? "";

            BuildUI();
        }

        private void BuildUI()
        {
            Text = "Similar Term Found";
            Font = new Font("Segoe UI", 9f);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            AutoSize = false;
            Size = new Size(520, 310);

            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 16, 20, 0)
            };

            // --- "You are adding:" label ---
            var addingLabel = new Label
            {
                Text = "You are adding:",
                AutoSize = true,
                Location = new Point(20, 16)
            };
            contentPanel.Controls.Add(addingLabel);

            // --- New term (bold) ---
            var newTermLabel = new Label
            {
                Text = $"  {_newSource}  \u2192  {_newTarget}",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 36),
                MaximumSize = new Size(464, 0)
            };
            contentPanel.Controls.Add(newTermLabel);

            // --- Separator ---
            var sep1 = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Height = 1,
                Location = new Point(20, 62),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Width = 464
            };
            contentPanel.Controls.Add(sep1);

            // --- Match description ---
            var match = _matches[0];
            string matchDescription;
            string synonymAction;

            if (match.MatchType == "source")
            {
                matchDescription = $"The source term \u201c{match.SourceTerm}\u201d already exists " +
                    $"with target \u201c{match.TargetTerm}\u201d";
                synonymAction = $"Add \u201c{_newTarget}\u201d as a target synonym " +
                    $"to the existing entry?";
            }
            else
            {
                matchDescription = $"The target term \u201c{match.TargetTerm}\u201d already exists " +
                    $"with source \u201c{match.SourceTerm}\u201d";
                synonymAction = $"Add \u201c{_newSource}\u201d as a source synonym " +
                    $"to the existing entry?";
            }

            // Termbase name
            matchDescription += $"\nin termbase \u201c{match.TermbaseName}\u201d.";

            // If there are matches in other termbases too, add a note
            int additionalCount = _matches.Count - 1;
            if (additionalCount > 0)
            {
                matchDescription += $"\n(and {additionalCount} more " +
                    $"{(additionalCount == 1 ? "match" : "matches")} in other termbases)";
            }

            var matchLabel = new Label
            {
                Text = matchDescription,
                Location = new Point(20, 72),
                Size = new Size(464, 60),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            contentPanel.Controls.Add(matchLabel);

            // --- Action question ---
            var actionLabel = new Label
            {
                Text = synonymAction,
                Location = new Point(20, 140),
                Size = new Size(464, 36),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9f, FontStyle.Italic)
            };
            contentPanel.Controls.Add(actionLabel);

            Controls.Add(contentPanel);

            // --- Bottom button bar ---
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 52
            };

            var sep2 = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Height = 1,
                Dock = DockStyle.Top
            };
            bottomPanel.Controls.Add(sep2);

            // Buttons: right-aligned, right to left: Cancel, Keep Both, Add & Edit..., Add as Synonym
            var btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(420, 12)
            };

            var btnKeepBoth = new Button
            {
                Text = "Keep Both",
                Size = new Size(90, 30),
                DialogResult = DialogResult.No,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(322, 12)
            };

            var btnEditReview = new Button
            {
                Text = "Add && Edit\u2026",
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(214, 12)
            };
            btnEditReview.Click += (s, e) =>
            {
                DialogResult = DialogResult.Retry;
                Close();
            };

            var btnMerge = new Button
            {
                Text = "Add as Synonym",
                Size = new Size(120, 30),
                DialogResult = DialogResult.Yes,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(86, 12)
            };

            bottomPanel.Controls.Add(btnMerge);
            bottomPanel.Controls.Add(btnEditReview);
            bottomPanel.Controls.Add(btnKeepBoth);
            bottomPanel.Controls.Add(btnCancel);

            Controls.Add(bottomPanel);

            AcceptButton = btnMerge;
            CancelButton = btnCancel;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
