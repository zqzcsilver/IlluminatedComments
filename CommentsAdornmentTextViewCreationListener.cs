using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace IlluminatedComments
{
    /// <summary>
    ///     Establishes an <see cref="IAdornmentLayer" /> to place the adornment on and exports the
    ///     <see cref="IWpfTextViewCreationListener" />
    ///     that instantiates the adornment on the event of a <see cref="IWpfTextView" />'s creation
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(ContentTypes.Cpp)]
    [ContentType(ContentTypes.CSharp)]
    [ContentType(ContentTypes.VisualBasic)]
    [ContentType(ContentTypes.FSharp)]
    [ContentType(ContentTypes.JavaScript)]
    [ContentType(ContentTypes.TypeScript)]
    [ContentType(ContentTypes.Python)]
    [ContentType(ContentTypes.Java)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class CommentsAdornmentTextViewCreationListener : IWpfTextViewCreationListener
    {
        // 禁用“字段从不分配给...”和“字段从不使用”编译器的警告。理由：该字段由 MEF 使用。
#pragma warning disable 649, 169

        /// <summary>
        ///     定义装饰的装饰层。此图层是有序的
        ///     在 Z 顺序的选择图层之后
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("CommentImageAdornmentLayer")]
        [Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
        private AdornmentLayerDefinition editorAdornmentLayer;

#pragma warning restore 649, 169

        [Import] internal SVsServiceProvider ServiceProvider;

        [Import] public ITextDocumentFactoryService TextDocumentFactory { get; set; }

        #region IWpfTextViewCreationListener

        /// <summary>
        ///     在具有匹配内容类型的文本数据模型上创建具有匹配角色的文本视图时调用。
        ///     在创建 textView 时实例化 CommentsAdornment 管理器。
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView" /> upon which the adornment should be placed</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            textView.Properties.GetOrCreateSingletonProperty(() => new CommentsAdornment(textView, TextDocumentFactory, ServiceProvider));
        }

        #endregion IWpfTextViewCreationListener
    }
}