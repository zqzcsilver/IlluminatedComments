using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IlluminatedComments.Patches
{
    internal class CompletionSourcePatch
    {
        public static void GetDescriptionAsyncPostfix(ICompletionSource __instance, IAsyncCompletionSession session, CancellationToken cancellationToken, ref Task<object> __result)
        {
            var r = __result;
            __result = Ssp(r, cancellationToken, session.TextView as IWpfTextView);
        }

        private static async Task<object> Ssp(Task<object> origin, CancellationToken cancellationToken, IWpfTextView textView)
        {
            object obj = await origin.ConfigureAwait(false);
            List<object> list = new List<object>();
            if (obj is ContainerElement containerElement)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
                AsyncQuickInfoSessionPatch.ConvertCommentImage(list, containerElement, textView);
                return list[0];
            }
            return obj;
        }
    }
}