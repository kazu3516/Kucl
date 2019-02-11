using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kucl.Xml;
using Kucl.Xml.XmlCfg;

namespace Kucl.Test {
    public class AppInfo : IUseConfig {

        private UseConfigHelper m_ConfigHelper;


        #region フィールド(メンバ変数、プロパティ、イベント)

        #region Name
        private string m_Name;
        /// <summary>
        /// Nameを取得、設定します。
        /// </summary>
        public string Name {
            get {
                return this.m_Name;
            }
            set {
                this.m_Name = value;
            }
        }
        #endregion


        #region Location
        private System.Drawing.Point m_Location;
        /// <summary>
        /// Locationを取得、設定します。
        /// </summary>
        public System.Drawing.Point Location {
            get {
                return this.m_Location;
            }
            set {
                this.m_Location = value;
            }
        }
        #endregion


        #region EnableDetailSettings
        private bool m_EnableDetailSettings;
        /// <summary>
        /// EnableDetailSettingsを取得、設定します。
        /// </summary>
        public bool EnableDetailSettings {
            get {
                return this.m_EnableDetailSettings;
            }
            set {
                this.m_EnableDetailSettings = value;
            }
        }
        #endregion


        public bool UnManagedMode {
            get; set;
        }

        #endregion

        #region コンストラクタ
        public AppInfo() {
            this.m_ConfigHelper = new UseConfigHelper(this.CreateDefaultConfig());

        }
        #endregion

        #region イベントハンドラ

        #endregion

        #region IUseConfig
        /// <summary>
        /// configとして保存するデフォルト設定を作成します。
        /// </summary>
        /// <returns></returns>
        public XmlConfigModel CreateDefaultConfig() {
            XmlConfigModel config = new XmlConfigModel();
            this.OnCreateDefaultConfig(config);
            return config;
        }
        /// <summary>
        /// 使用するConfigを取得または設定します。
        /// </summary>
        public XmlConfigModel Config {
            get {
                return this.m_ConfigHelper.Config;
            }
            set {
                this.m_ConfigHelper.Config = value;
            }
        }
        /// <summary>
        /// configを読み込んで適用します。
        /// </summary>
        /// <param name="value"></param>
        public void ApplyConfig(XmlConfigModel value) {
            this.m_ConfigHelper.Config = value;
            this.OnApplyConfig(value);
        }
        /// <summary>
        /// 現在の設定をconfigに反映します。
        /// </summary>
        /// <param name="config"></param>
        public void ReflectConfig(XmlConfigModel config) {
            this.OnReflectConfig(config);
        }
        /// <summary>
        /// configの値がデフォルト値かどうかを判定します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsDefaultValue(string name, XmlContentsItem value) {
            return this.m_ConfigHelper.IsDefaultValue(name, value);
        }
        #endregion

        #region Configの適用と更新

        //Configの適用
        private void OnApplyConfig(XmlConfigModel config) {

            this.m_Name = this.m_ConfigHelper.GetStringValue("Kucl.setting:Common.Name");

            int x = this.m_ConfigHelper.GetIntValue("Kucl.setting:Common.Location.X");
            int y = this.m_ConfigHelper.GetIntValue("Kucl.setting:Common.Location.Y");
            this.m_Location = new System.Drawing.Point(x, y);

            this.m_EnableDetailSettings = this.m_ConfigHelper.GetBoolValue("Kucl.setting:Common.Settings.Detail.Enabled");

            this.UnManagedMode = this.m_ConfigHelper.GetBoolValue("Kucl.setting:Special.Settings.UnManaged");
        }


        //Configの更新
        private void OnReflectConfig(XmlConfigModel config) {
            config.AddXmlContentsItem("Kucl.setting:Common.Name", this.m_Name);
            config.AddXmlContentsItem("Kucl.setting:Common.Location.X", this.m_Location.X);
            config.AddXmlContentsItem("Kucl.setting:Common.Location.Y", this.m_Location.Y);
            config.AddXmlContentsItem("Kucl.setting:Common.Settings.Detail.Enabled", this.m_EnableDetailSettings);

            config.AddXmlContentsItem("Kucl.setting:Special.Settings.UnManaged", this.UnManagedMode);
        }

        //既定のConfig
        private void OnCreateDefaultConfig(XmlConfigModel config) {
            config.AddXmlContentsItem("Kucl.setting:Common.Name", "SAMPLE_NAME");
            config.AddXmlContentsItem("Kucl.setting:Common.Location.X", 10);
            config.AddXmlContentsItem("Kucl.setting:Common.Location.Y", 20);
            config.AddXmlContentsItem("Kucl.setting:Common.Settings.Detail.Enabled", false);
            config.AddXmlContentsItem("Kucl.setting:Special.Settings.UnManaged", false);
        }


        #endregion

    }
}
