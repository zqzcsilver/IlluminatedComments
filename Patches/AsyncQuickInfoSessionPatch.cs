using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IlluminatedComments.Patches
{
    internal class AsyncQuickInfoSessionPatch
    {
        private static readonly List<string> _processingUris = new List<string>();
        private static readonly ConcurrentDictionary<WebClient, ImageParameters> _toaddImages = new ConcurrentDictionary<WebClient, ImageParameters>();

        public static void ComputeContentAsyncPostfix(IAsyncQuickInfoSession __instance, CancellationToken cancellationToken, ref Task<(IList<object> items, IList<ITrackingSpan> applicableToSpans)> __result)
        {
            var r = __result;
            __result = Ssp(r, __instance, cancellationToken);
        }

        private static async Task<(IList<object> items, IList<ITrackingSpan> applicableToSpans)> Ssp(Task<(IList<object>, IList<ITrackingSpan>)> task, IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var result = await task.ConfigureAwait(false);
            List<object> list = new List<object>();
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            for (int i = 0; i < result.Item1.Count; i++)
            {
                if (result.Item1[i] is ContainerElement containerElement)
                {
                    ConvertCommentImage(list, containerElement, session.TextView as IWpfTextView);
                    result.Item1[i] = list[0];
                    list.Clear();
                }
            }
            return result;
        }

        public static void ConvertCommentImage(List<object> list, ContainerElement containerElement, IWpfTextView view)
        {
            List<object> myList = new List<object>();
            foreach (var item in containerElement.Elements)
            {
                if (item is ContainerElement container)
                {
                    ConvertCommentImage(myList, container, view);
                }
                else if (item is ClassifiedTextElement element)
                {
                    StringBuilder str = new StringBuilder();
                    foreach (var run in element.Runs)
                    {
                        str.Append(run.Text);
                    }
                    if (CommentImageParser.TryParse(str.ToString(), out string imageUrl, out double imageScale, out _))
                    {
                        var v = new VariableExpander(view, CommentLineTransformSourceProvider.SVsServiceProvider);
                        CommentImage image = new CommentImage(v);

                        var originalUrl = imageUrl;

                        if (imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        {
                            if (ImageCache.Instance.TryGetValue(imageUrl, out var localPath))
                            {
                                imageUrl = localPath;
                            }
                            else
                            {
                                var tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(imageUrl));
                                var client = new WebClient();
                                client.DownloadDataCompleted += Client_DownloadDataCompleted; ;

                                _toaddImages.TryAdd(
                                    client,
                                    new ImageParameters
                                    {
                                        Uri = imageUrl,
                                        LocalPath = tempPath,
                                        Image = image,
                                        Scale = imageScale
                                    });

                                client.DownloadDataAsync(new Uri(imageUrl));

                                return;
                            }
                        }

                        if (imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                            if (ImageCache.Instance.TryGetValue(imageUrl, out var localPath))
                                imageUrl = localPath;

                        if (image.TrySet(imageUrl, originalUrl, imageScale, CommentLineTransformSourceProvider.CodeFilePath, out _))
                            myList.Add(image);
                        else
                            myList.Add(item);
                    }
                    else
                        myList.Add(item);
                }
                else
                    myList.Add(item);
            }
            list.Add(new ContainerElement(containerElement.Style, myList));
        }

        private static void Client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                var client = sender as WebClient;

                client.DownloadDataCompleted -= Client_DownloadDataCompleted;

                if (_toaddImages.TryGetValue(client, out var item))
                {
                    var data = e.Result;
                    File.WriteAllBytes(item.LocalPath, data);
                    ImageCache.Instance.Add(item.Uri, item.LocalPath);
                    _processingUris.Remove(item.Uri);
                    _toaddImages.TryRemove(client, out var value);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Notify(ex, true);
            }
        }

        private class ImageParameters
        {
            public string Uri { get; set; }
            public string LocalPath { get; set; }
            public CommentImage Image { get; set; }
            public double Scale { get; set; }
        }
    }
}