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


        }


        //Configの更新
        private void OnReflectConfig(XmlConfigModel config) {
            config.AddXmlContentsItem("Kucl.setting:Common.Name", this.m_Name);


        }

        //既定のConfig
        private void OnCreateDefaultConfig(XmlConfigModel config) {
            config.AddXmlContentsItem("Kucl.setting:Common.Name", "SAMPLE_NAME");


        }


        #endregion

    }
}
