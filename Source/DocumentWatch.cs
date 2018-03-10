// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;
    using System.IO;
    using System.Windows.Threading;

    internal class DocumentWatch
    {
        private readonly FileSystemWatcher watcher = new FileSystemWatcher();
        private readonly DispatcherTimer reloadTimer = new DispatcherTimer();
        private FileModel fileModel;
        private uint reloadRetryCounter;

        public DocumentWatch()
        {
            this.watcher.NotifyFilter = NotifyFilters.LastWrite;
            this.watcher.Changed += (s, e) =>
            {
                this.reloadTimer.Stop();
                this.reloadRetryCounter = 0;
                this.reloadTimer.Start();
            };

            this.reloadTimer.Interval = TimeSpan.FromSeconds(1);
            this.reloadTimer.Tick += (s, e) =>
            {
                this.reloadTimer.Stop();

                if (this.reloadRetryCounter >= 3)
                {
                    return;
                }

                if (this.IsFileAccessible() == false)
                {
                    this.reloadRetryCounter++;
                    this.reloadTimer.Start();
                }

                this.OnShouldReload();
            };
        }

        public event EventHandler ShouldReload;

        public void MonitorFile(FileModel model)
        {
            this.StopMonitoring();

            if (string.IsNullOrEmpty(model?.FullName))
            {
                return;
            }

            this.fileModel = model;
            this.watcher.Path = this.fileModel.Path;
            this.watcher.Filter = this.fileModel.Name;
            this.watcher.EnableRaisingEvents = true;
        }

        public void StopMonitoring()
        {
            this.reloadTimer.Stop();
            this.watcher.EnableRaisingEvents = false;
        }

        protected virtual void OnShouldReload()
        {
            this.ShouldReload?.Invoke(this, EventArgs.Empty);
        }

        private bool IsFileAccessible()
        {
            FileStream stream = null;

            try
            {
                stream = new FileInfo(this.fileModel.FullName).Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                // - still being written to
                // - or being processed by another thread
                // - or does not exist
                return false;
            }
            finally
            {
                stream?.Close();
            }

            return true;
        }
    }
}