using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PortableLauncher
{
    /// <summary>
    /// 設定全体のクラスです
    /// </summary>
    public class LauncherSetting
    {
        // 以下、List内に追加される型を設定する
        [System.Xml.Serialization.XmlArrayItem(typeof(SettingItem)),
            System.Xml.Serialization.XmlArrayItem(typeof(int)),
            System.Xml.Serialization.XmlArrayItem(typeof(string))]
        public List<SettingItem> items = new List<SettingItem>();
        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public LauncherSetting()
        {

        }
    }

    /// <summary>
    /// 設定のうち、アプリケーションと言った項目のクラスです
    /// </summary>
    public class SettingItem
    {
        /// <summary>
        /// アイテムのタイプ
        /// 0:セパレータ, 1:アプリケーション, 2:フォルダ
        /// </summary>
        private int type;
        /// <summary>
        /// アプリケーションのパス
        /// </summary>
        public String appPath;
        /// <summary>
        /// アプリケーションの名前
        /// </summary>
        public String appName;
        /// <summary>
        /// コンストラクタです
        /// </summary>
        /// <param name="type">設定するタイプ タイプ設定後は固定です</param>
        public SettingItem(int type)
        {
            this.type = type;
        }
        /// <summary>
        /// タイプを確認します
        /// </summary>
        public int checkType()
        {
            return this.type;
        }
        /// <summary>
        /// アプリケーション用として設定します
        /// </summary>
        /// <param name="appPath">アプリケーションのパス</param>
        /// <param name="appName">アプリケーションの名前</param>
        public void setApp(String appPath, String appName)
        {
            if (type != 1)
            {
                return;
            }
            this.appPath = appPath;
            this.appName = appName;
        }
    }
}
