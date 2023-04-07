using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using Exquisite.UI.Utils;
using System.Timers;
using System.Windows.Threading;
using System.Globalization;

namespace Exquisite.ViewModels
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive
    {
        private readonly IWindowManager _windowManager;
        public string CurrentTime { get; set; }
        private readonly DispatcherTimer timer;

        public ShellViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;

            // 创建一个每秒钟更新一次的 DispatcherTimer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // 更新当前时间
            CultureInfo cultureInfo = new CultureInfo("zh-CN");
            CurrentTime = DateTime.Now.ToString("yyyy年M月d日 dddd HH:mm:ss", cultureInfo);
        }


        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            //初始化界面
            await Navigate(IoC.Get<ACViewModel>("AC"));
        }

        public async void OnMainMenuSelectionChanged(object listboxitem)
        {
            string viewname = string.Empty;

            if (listboxitem as ListBoxItem != null)
            {
                TextBlock textBlock = WpfUtils.FindVisualChild<TextBlock>(listboxitem as ListBoxItem);
                if (textBlock != null)
                {
                    viewname = textBlock.Text;
                }
            }



            //viewname = Regex.Match(viewname, @"[\S^:]*$").Value;//使用正则表达式匹配冒号后的所有非空字符

            //viewname = viewname.Substring(viewname.IndexOf(':') + 1).Trim();
            switch (viewname)
            {
                case "AC": await Navigate(IoC.Get<ACViewModel>("AC")); break;
                case "DC": await Navigate(IoC.Get<DCViewModel>("DC")); break;

            }
        }

        public async Task Navigate(Screen viewmodel)
        {
            await ActivateItemAsync(viewmodel, new CancellationToken());

        }

        
    }
}
