using Exquisite.Shared.Components;

namespace Exquisite.ViewModels;

public class AcViewModel : ViewModelBase
{
	public AcViewModel()
	{
		for (int i = 0; i < 50; i++)
		{
			Logger.Instance.Debug($"这是来自AC的第{i}条消息");
			Logger.Instance.Error($"这是来自AC的第{i}条错误");
		}
	}
}