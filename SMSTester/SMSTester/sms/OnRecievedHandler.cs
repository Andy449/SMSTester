using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuctech.RDP.Peripherals
{
    /// <summary>
    /// 创建事件收到信息的委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void OnRecievedHandler(object sender, RecievedSMS e);


}
