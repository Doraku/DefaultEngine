using System.Threading;
using System.Threading.Tasks;

namespace DefaultEngine.Editor.Api.Services;

public interface IContentDialogService
{
    enum DialogResult
    {
        None,
        Primary,
        Secondary
    }

    Task<DialogResult> ShowAsync(object content, CancellationToken cancellationToken = default);
}
