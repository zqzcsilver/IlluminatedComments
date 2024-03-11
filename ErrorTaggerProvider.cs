using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace IlluminatedComments
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType(ContentTypes.Cpp)]
    [ContentType(ContentTypes.CSharp)]
    [ContentType(ContentTypes.VisualBasic)]
    [ContentType(ContentTypes.FSharp)]
    [ContentType(ContentTypes.JavaScript)]
    [ContentType(ContentTypes.TypeScript)]
    [ContentType(ContentTypes.Python)]
    [ContentType(ContentTypes.Java)]
    [TagType(typeof(ErrorTag))]
    internal class ErrorTaggerProvider : IViewTaggerProvider
    {
        [Import] internal SVsServiceProvider ServiceProvider;

        [Import] public ITextDocumentFactoryService TextDocumentFactory { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer)
            where T : ITag
        {
            if (textView == null) return null;

            if (textView.TextBuffer != buffer) return null;

            Trace.Assert(textView is IWpfTextView);

            var imageAdornmentManager = textView.Properties.GetOrCreateSingletonProperty(() => new CommentsAdornment((IWpfTextView) textView, TextDocumentFactory, ServiceProvider));
            return imageAdornmentManager as ITagger<T>;
        }
    }
}