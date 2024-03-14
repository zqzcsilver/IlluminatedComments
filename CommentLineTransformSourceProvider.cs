using IlluminatedComments.Patches;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IlluminatedComments
{
    [Export(typeof(ILineTransformSourceProvider))]
    [ContentType(ContentTypes.Cpp)]
    [ContentType(ContentTypes.CSharp)]
    [ContentType(ContentTypes.VisualBasic)]
    [ContentType(ContentTypes.FSharp)]
    [ContentType(ContentTypes.JavaScript)]
    [ContentType(ContentTypes.TypeScript)]
    [ContentType(ContentTypes.Python)]
    [ContentType(ContentTypes.Java)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal class CommentLineTransformSourceProvider : ILineTransformSourceProvider
    {
        public static SVsServiceProvider SVsServiceProvider;

        public CommentLineTransformSourceProvider()
        {
        }

        [Import] internal SVsServiceProvider ServiceProvider;

        [Import] public ITextDocumentFactoryService TextDocumentFactory { get; set; }

        public static string CodeFilePath = string.Empty;

        ILineTransformSource ILineTransformSourceProvider.Create(IWpfTextView view)
        {
            SVsServiceProvider = ServiceProvider;
            if (TextDocumentFactory.TryGetTextDocument(view.TextBuffer, out var textDoc))
                CodeFilePath = textDoc.FilePath;
            else
                CodeFilePath = string.Empty;

            HarmonyLoader.VSEditorAssembly = view.GetType().Assembly;
            HarmonyLoader.Load();

            var manager = view.Properties.GetOrCreateSingletonProperty(() => new CommentsAdornment(view, TextDocumentFactory, ServiceProvider));
            return new CommentLineTransformSource(manager);
        }
    }
}