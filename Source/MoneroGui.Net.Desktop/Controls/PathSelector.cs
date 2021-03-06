﻿using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Jojatekok.MoneroGUI.Desktop.Controls
{
    public sealed class PathSelector : TableLayout
    {
        private readonly TextBox _textBoxPath = new TextBox();

        private TextBox TextBoxPath {
            get { return _textBoxPath; }
        }

        public string SelectedPath {
            get { return TextBoxPath.Text; }
            set { TextBoxPath.Text = value; }
        }

        public HashSet<FileDialogFilter> Filters { get; set; }

        public PathSelector()
        {
            Spacing = Utilities.Spacing3;

            var buttonSelectPath = new Button {
                Image = Utilities.LoadImage("Open")
            };
            buttonSelectPath.Click += OnButtonSelectPathClick;

            Rows.Add(
                new TableRow(
                    new TableCell(TextBoxPath, true),
                    buttonSelectPath
                )
            );
        }

        void OnButtonSelectPathClick(object sender, EventArgs e)
        {
            if (Filters != null) {
                // Handle file selection
                using (var dialog = new OpenFileDialog()) {
                    foreach (var filter in Filters) {
                        dialog.Filters.Add(filter);
                    }

                    if (TextBoxPath.Text.Length != 0) {
                        var fileInfo = new FileInfo(TextBoxPath.Text);
                        if (fileInfo.Exists) {
                            Debug.Assert(fileInfo.DirectoryName != null, "fileInfo.DirectoryName != null");
                            dialog.Directory = new Uri(fileInfo.DirectoryName);
                        } else {
                            dialog.Directory = new Uri(Utilities.ApplicationBaseDirectory);
                        }
                    }

                    if (dialog.ShowDialog(this) == DialogResult.Ok) {
                        TextBoxPath.Text = Utilities.GetRelativePath(dialog.FileName);
                    }
                }

            } else {
                // Handle directory selection
                using (var dialog = new SelectFolderDialog()) {
                    if (TextBoxPath.Text.Length != 0) {
                        var directory = Utilities.GetAbsolutePath(TextBoxPath.Text);
                        if (Directory.Exists(directory)) dialog.Directory = directory;
                    }

                    if (dialog.ShowDialog(this) == DialogResult.Ok) {
                        TextBoxPath.Text = Utilities.GetRelativePath(dialog.Directory);
                    }
                }
            }

            TextBoxPath.SelectAll();
            TextBoxPath.Focus();
        }
    }
}
