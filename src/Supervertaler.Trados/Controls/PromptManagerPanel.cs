using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Supervertaler.Trados.Core;
using Supervertaler.Trados.Models;
using Supervertaler.Trados.Settings;

namespace Supervertaler.Trados.Controls
{
    /// <summary>
    /// UserControl for the "Prompts" tab in the Settings dialog.
    /// Shows the base system prompt (read-only or editable) and the custom prompt library.
    /// </summary>
    public class PromptManagerPanel : UserControl
    {
        private TextBox _txtSystemPrompt;
        private Button _btnEditSystem;
        private Button _btnResetSystem;
        private Label _lblSystemStatus;

        private DataGridView _dgvPrompts;
        private Button _btnNew;
        private Button _btnEdit;
        private Button _btnDelete;
        private Button _btnRestore;

        private PromptLibrary _library;
        private List<PromptTemplate> _prompts;
        private string _customSystemPrompt; // null = use default

        public PromptManagerPanel()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            SuspendLayout();
            BackColor = Color.White;

            var y = 10;
            var labelColor = Color.FromArgb(80, 80, 80);
            var headerFont = new Font("Segoe UI", 9f, FontStyle.Bold);
            var bodyFont = new Font("Segoe UI", 8.5f);

            // ═══════════════════════════════════════════════
            // SYSTEM PROMPT SECTION
            // ═══════════════════════════════════════════════
            var lblSysHeader = new Label
            {
                Text = "System Prompt",
                Font = headerFont,
                ForeColor = Color.FromArgb(50, 50, 50),
                Location = new Point(10, y),
                AutoSize = true
            };
            Controls.Add(lblSysHeader);
            y += 22;

            var lblSysInfo = new Label
            {
                Text = "The system prompt provides base instructions for AI translation (tag preservation, " +
                    "number formatting, etc.). It is always included before any custom prompt.",
                Location = new Point(10, y),
                Font = new Font("Segoe UI", 7.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(130, 130, 130),
                AutoSize = false,
                Height = 28,
                Width = 500,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(lblSysInfo);
            y += 30;

            _txtSystemPrompt = new TextBox
            {
                Location = new Point(10, y),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 7.5f),
                BackColor = Color.FromArgb(248, 248, 248),
                ForeColor = Color.FromArgb(60, 60, 60),
                WordWrap = true,
                Height = 120,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(_txtSystemPrompt);
            y += 124;

            // Buttons row for system prompt
            _btnEditSystem = new Button
            {
                Text = "Edit System Prompt",
                Location = new Point(10, y),
                Width = 130,
                Height = 25,
                FlatStyle = FlatStyle.System,
                Font = bodyFont
            };
            _btnEditSystem.Click += OnEditSystemPrompt;

            _btnResetSystem = new Button
            {
                Text = "Reset to Default",
                Location = new Point(146, y),
                Width = 120,
                Height = 25,
                FlatStyle = FlatStyle.System,
                Font = bodyFont
            };
            _btnResetSystem.Click += OnResetSystemPrompt;

            _lblSystemStatus = new Label
            {
                Text = "",
                Location = new Point(274, y + 4),
                AutoSize = true,
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 8f)
            };

            Controls.Add(_btnEditSystem);
            Controls.Add(_btnResetSystem);
            Controls.Add(_lblSystemStatus);
            y += 36;

            // ═══════════════════════════════════════════════
            // CUSTOM PROMPT LIBRARY SECTION
            // ═══════════════════════════════════════════════
            var sep = new Label
            {
                Location = new Point(10, y),
                Height = 1,
                BorderStyle = BorderStyle.Fixed3D,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(sep);
            y += 8;

            var lblLibHeader = new Label
            {
                Text = "Custom Prompt Library",
                Font = headerFont,
                ForeColor = Color.FromArgb(50, 50, 50),
                Location = new Point(10, y),
                AutoSize = true
            };
            Controls.Add(lblLibHeader);

            // Buttons (right-aligned on header row)
            _btnRestore = new Button
            {
                Text = "Restore Built-in",
                Width = 105,
                Height = 25,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(80, 80, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnRestore.FlatAppearance.BorderSize = 0;
            _btnRestore.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 220, 220);
            _btnRestore.Click += OnRestoreBuiltIn;

            _btnDelete = new Button
            {
                Text = "Delete",
                Width = 55,
                Height = 25,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(80, 80, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnDelete.FlatAppearance.BorderSize = 0;
            _btnDelete.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 220, 220);
            _btnDelete.Click += OnDeletePrompt;

            _btnEdit = new Button
            {
                Text = "Edit",
                Width = 45,
                Height = 25,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(80, 80, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnEdit.FlatAppearance.BorderSize = 0;
            _btnEdit.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 220, 220);
            _btnEdit.Click += OnEditPrompt;

            _btnNew = new Button
            {
                Text = "New",
                Width = 45,
                Height = 25,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(80, 80, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnNew.FlatAppearance.BorderSize = 0;
            _btnNew.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 220, 220);
            _btnNew.Click += OnNewPrompt;

            Controls.Add(_btnRestore);
            Controls.Add(_btnDelete);
            Controls.Add(_btnEdit);
            Controls.Add(_btnNew);
            y += 28;

            // ─── Prompt grid ──────────────────────────────
            _dgvPrompts = new DataGridView
            {
                Location = new Point(10, y),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackgroundColor = Color.FromArgb(250, 250, 250),
                Font = new Font("Segoe UI", 8.5f),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                EnableHeadersVisualStyles = false
            };
            _dgvPrompts.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.FromArgb(50, 50, 50),
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                SelectionBackColor = Color.FromArgb(240, 240, 240),
                SelectionForeColor = Color.FromArgb(50, 50, 50)
            };
            _dgvPrompts.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(250, 250, 250),
                ForeColor = Color.FromArgb(40, 40, 40),
                SelectionBackColor = Color.FromArgb(220, 235, 252),
                SelectionForeColor = Color.FromArgb(40, 40, 40)
            };

            var colName = new DataGridViewTextBoxColumn
            {
                Name = "colName",
                HeaderText = "Name",
                FillWeight = 50
            };
            var colDomain = new DataGridViewTextBoxColumn
            {
                Name = "colDomain",
                HeaderText = "Category",
                FillWeight = 25
            };
            var colSource = new DataGridViewTextBoxColumn
            {
                Name = "colSource",
                HeaderText = "Source",
                Width = 65,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                FillWeight = 1
            };
            _dgvPrompts.Columns.AddRange(new DataGridViewColumn[] { colName, colDomain, colSource });
            _dgvPrompts.CellDoubleClick += OnGridDoubleClick;

            Controls.Add(_dgvPrompts);

            ResumeLayout(false);

            // Handle resize to position elements
            Resize += OnResize;
        }

        private void OnResize(object sender, EventArgs e)
        {
            var w = Width - 20;

            // System prompt textbox width
            if (_txtSystemPrompt != null)
                _txtSystemPrompt.Width = Math.Max(100, w);

            // Separator width
            foreach (Control c in Controls)
            {
                if (c is Label lbl && lbl.BorderStyle == BorderStyle.Fixed3D)
                    lbl.Width = Math.Max(100, w);
            }

            // Button row positioning for library section
            if (_btnRestore != null)
            {
                _btnRestore.Location = new Point(Width - 10 - _btnRestore.Width, _btnRestore.Top);
                _btnDelete.Location = new Point(_btnRestore.Left - _btnDelete.Width - 2, _btnDelete.Top);
                _btnEdit.Location = new Point(_btnDelete.Left - _btnEdit.Width - 2, _btnEdit.Top);
                _btnNew.Location = new Point(_btnEdit.Left - _btnNew.Width - 2, _btnNew.Top);
            }

            // Grid width
            if (_dgvPrompts != null)
                _dgvPrompts.Width = Math.Max(100, w);
        }

        // ─── Public API ─────────────────────────────────────────

        /// <summary>
        /// Populates the panel from current settings and prompt library.
        /// </summary>
        public void PopulateFromSettings(AiSettings settings, PromptLibrary library)
        {
            _library = library ?? new PromptLibrary();
            _customSystemPrompt = settings?.CustomSystemPrompt;

            // Show system prompt
            UpdateSystemPromptDisplay();

            // Load prompt library
            RefreshPromptList();
        }

        /// <summary>
        /// Applies changes back to AI settings.
        /// </summary>
        public void ApplyToSettings(AiSettings settings)
        {
            if (settings == null) return;
            settings.CustomSystemPrompt = _customSystemPrompt;
        }

        // ─── System Prompt ──────────────────────────────────────

        private void UpdateSystemPromptDisplay()
        {
            if (!string.IsNullOrWhiteSpace(_customSystemPrompt))
            {
                _txtSystemPrompt.Text = _customSystemPrompt;
                _lblSystemStatus.Text = "(customized)";
                _lblSystemStatus.ForeColor = Color.FromArgb(180, 120, 0);
            }
            else
            {
                _txtSystemPrompt.Text = TranslationPrompt.GetDefaultBaseSystemPrompt();
                _lblSystemStatus.Text = "(default)";
                _lblSystemStatus.ForeColor = Color.FromArgb(30, 130, 60);
            }
        }

        private void OnEditSystemPrompt(object sender, EventArgs e)
        {
            var content = !string.IsNullOrWhiteSpace(_customSystemPrompt)
                ? _customSystemPrompt
                : TranslationPrompt.GetDefaultBaseSystemPrompt();

            var prompt = new PromptTemplate
            {
                Name = "System Prompt",
                Description = "Base system instructions for AI translation",
                Domain = "System",
                Content = content
            };

            using (var dlg = new PromptEditorDialog(prompt))
            {
                dlg.Text = "Edit System Prompt";
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _customSystemPrompt = dlg.Result.Content;
                    UpdateSystemPromptDisplay();
                }
            }
        }

        private void OnResetSystemPrompt(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Reset the system prompt to the default?\n\nThis will discard any customizations.",
                "Reset System Prompt",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                _customSystemPrompt = null;
                UpdateSystemPromptDisplay();
            }
        }

        // ─── Custom Prompt Library ──────────────────────────────

        private void RefreshPromptList()
        {
            _dgvPrompts.Rows.Clear();
            _prompts = _library.GetAllPrompts();

            foreach (var p in _prompts)
            {
                var source = p.IsReadOnly ? "Supervertaler" : (p.IsBuiltIn ? "Built-in" : "Custom");
                _dgvPrompts.Rows.Add(p.Name, p.Domain, source);
            }
        }

        private PromptTemplate GetSelectedPrompt()
        {
            if (_dgvPrompts.SelectedRows.Count == 0)
                return null;
            var idx = _dgvPrompts.SelectedRows[0].Index;
            if (idx < 0 || idx >= _prompts.Count)
                return null;
            return _prompts[idx];
        }

        private void OnNewPrompt(object sender, EventArgs e)
        {
            using (var dlg = new PromptEditorDialog())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _library.SavePrompt(dlg.Result);
                    RefreshPromptList();
                }
            }
        }

        private void OnEditPrompt(object sender, EventArgs e)
        {
            var selected = GetSelectedPrompt();
            if (selected == null)
            {
                MessageBox.Show("Select a prompt to edit.",
                    "Prompts", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dlg = new PromptEditorDialog(selected))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _library.SavePrompt(dlg.Result);
                    RefreshPromptList();
                }
            }
        }

        private void OnDeletePrompt(object sender, EventArgs e)
        {
            var selected = GetSelectedPrompt();
            if (selected == null)
            {
                MessageBox.Show("Select a prompt to delete.",
                    "Prompts", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selected.IsReadOnly)
            {
                MessageBox.Show("This prompt is from the Supervertaler desktop app and cannot be deleted from here.",
                    "Prompts", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Delete prompt \"{selected.Name}\"?\n\nThis cannot be undone.",
                "Delete Prompt",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                _library.DeletePrompt(selected);
                RefreshPromptList();
            }
        }

        private void OnRestoreBuiltIn(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Restore all built-in prompts?\n\nThis will overwrite any edits to built-in prompts and re-create deleted ones.",
                "Restore Built-in Prompts",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                _library.RestoreBuiltInPrompts();
                RefreshPromptList();
            }
        }

        private void OnGridDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            OnEditPrompt(sender, EventArgs.Empty);
        }
    }
}
