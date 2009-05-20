﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using UnRarIt.Interop;

namespace UnRarIt
{
    internal class ArchiveItem : ListViewItem
    {
        private static Properties.Settings Config = Properties.Settings.Default;

        Dictionary<string, bool> parts = new Dictionary<string, bool>();
        FileInfo file = null;
        public ArchiveItem(string aFileName)
        {
            SubItems.Add(string.Empty);
            SubItems.Add("Ready...");
            file = new FileInfo(aFileName);
            Invalidate();
        }

        protected void Invalidate()
        {
            Text = file.Name + (parts.Count != 0 ? String.Format(" +{0} parts", parts.Count) : "");

            if (file.Exists)
            {
                FileSize = Main.ToFormatedSize(file.Length);
            }
            else
            {
                FileSize = "n/a";
            }
        }
        private new string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public string FileName
        {
            get { return file.FullName; }
            set { file = new FileInfo(value); Invalidate(); }
        }
        public string FileSize
        {
            get { return SubItems[1].Text; }
            private set { SubItems[1].Text = value; }
        }
        public string Status
        {
            get { return SubItems[2].Text; }
            set { SubItems[2].Text = value; }
        }
        public FileInfo File
        {
            get { return file; }
        }

        internal void AddPart(string Part)
        {
            parts[Part] = true;
            Invalidate();
        }

        internal void ExcuteSuccessAction()
        {
            switch (Config.SuccessAction)
            {
                case 1:
                    Rename(file.FullName);
                    foreach (string part in parts.Keys)
                    {
                        Rename(part);
                    }
                    break;
                case 2:
                    file.Delete();
                    foreach (string part in parts.Keys)
                    {
                        new FileInfo(part).Delete();
                    }
                    break;
            }
        }
        internal static void Rename(string aFile)
        {
            FileInfo fi = new FileInfo(aFile);
            fi.MoveTo(Reimplement.CombinePath(fi.Directory.FullName, String.Format("unrarit_{0}", fi.Name)));
        }

    }
}