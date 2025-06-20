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

    interface INone
    {
        object? NoneContent { get; }
    }

    interface IPrimary
    {
        object? PrimaryContent { get; }

        bool CanReturnPrimary => true;
    }

    interface ISecondary
    {
        object? SecondaryContent { get; }

        bool CanReturnSecondary => true;
    }

    Task<DialogResult> ShowAsync(object content, bool isFullScreen = false, CancellationToken cancellationToken = default);
}
