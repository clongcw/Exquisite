using Exquisite.Shared.Components;

namespace Exquisite.ViewModels;

public class DcViewModel : ViewModelBase
{
	public DcViewModel()
	{
        for (int i = 0; i < 50; i++)
        {
            Logger.Instance.Debug("这是来自DC的第{0}条{1}消息",i,i);
            Logger.Instance.Error($"这是来自DC的第{i}条错误");
        }
    }
}