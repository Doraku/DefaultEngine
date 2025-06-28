using System.Threading;
using System.Threading.Tasks;
using DefaultApplication.Services;

namespace DefaultApplication.Internal.Plugins.ContentDialogServicePlugin.Services;

internal sealed class NoApplicationContentDialogService : IContentDialogService
{
    public Task<IContentDialogService.DialogResult> ShowAsync(object content, CancellationToken cancellationToken = default) => Task.FromResult(IContentDialogService.DialogResult.None);
}
