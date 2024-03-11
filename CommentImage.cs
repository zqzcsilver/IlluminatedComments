﻿using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using WpfAnimatedGif;

namespace IlluminatedComments
{
    public class CommentImage : Image, IDisposable
    {
        private readonly FileSystemWatcher fileSystemWatcher = new FileSystemWatcher
        {
            IncludeSubdirectories = false,
            NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.Attributes
        };

        private double _scale;
        private readonly VariableExpander _variableExpander;

        public CommentImage(VariableExpander variableExpander)
        {
            _variableExpander = variableExpander ?? throw new ArgumentNullException(nameof(variableExpander));

            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Renamed += FileSystemWatcher_Changed;
        }

        /// <summary>
        ///     Location from where the image was loaded.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        ///     Original location from which the image was downloaded.
        /// </summary>
        public string OriginalUrl { get; private set; }

        /// <summary>
        ///     Scale image if value is greater than 0, otherwise use source dimensions
        /// </summary>
        public double Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                if (Source != null)
                {
                    if (value > 0)
                    {
                        Width = Source.Width * value;
                        Height = Source.Height * value;
                    }
                    else
                    {
                        Width = Source.Width;
                        Height = Source.Height;
                    }
                }
            }
        }

        public void Dispose()
        {
            fileSystemWatcher.Changed -= FileSystemWatcher_Changed;
            fileSystemWatcher.Renamed -= FileSystemWatcher_Changed;
            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Dispose();
        }

        public bool TrySet(string imageUrl, string originalUrl, double scale, string filepath, out Exception exception)
        {
            exception = null;
            try
            {
                imageUrl = _variableExpander.ProcessText(imageUrl);
                Source = LoadImage(imageUrl, filepath);
                Url = imageUrl;
                OriginalUrl = originalUrl;
                Scale = scale;

                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        private bool IsAbsoluteUri(string uri) => Uri.TryCreate(uri, UriKind.Absolute, out var result);

        private BitmapImage LoadImage(string uri, string filepath)
        {
            if (!IsAbsoluteUri(uri)) uri = Path.Combine(Path.GetDirectoryName(filepath), uri);

            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Path = Path.GetDirectoryName(uri);
            fileSystemWatcher.Filter = Path.GetFileName(uri);
            fileSystemWatcher.EnableRaisingEvents = true;

            if (!File.Exists(uri)) return null;

            var image = new BitmapImage
            {
                CacheOption = BitmapCacheOption.OnLoad
            };

            image.BeginInit();
            image.UriSource = new Uri(uri, UriKind.Absolute);
            image.EndInit();

            if (!image.IsDownloading)
            {
                if (image.CanFreeze)
                    image.Freeze();
            }
            else
            {
                image.DownloadCompleted += (s, e) =>
                {
                    if ((s as BitmapImage).CanFreeze)
                        (s as BitmapImage).Freeze();
                };
            }

            if (string.Equals(Path.GetExtension(uri), ".gif", StringComparison.OrdinalIgnoreCase))
                ImageBehavior.SetAnimatedSource(this, image);

            return image;
        }

        public override string ToString() => Url;

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e) => Dispatcher.Invoke(() =>
        {
            Source = LoadImage(e.FullPath, string.Empty);
            Scale = _scale;
            InvalidateVisual();
        });
    }
}