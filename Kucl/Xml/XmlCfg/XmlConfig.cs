using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kucl.Xml;

//Ver 1.0.7.0で追加
//Kucl.Cfgパッケージを拡張してXmlContentsパッケージとした。
//IUseConfigインターフェースやUseConfigHelperはXmlContentsを使用するクラスとして移植。
//その他のKucl.Cfgパッケージは非推奨としてマーク。
//
//XmlContentsクラス群を拡張してXmlConfigクラス群として再定義
//XmlContentsItemとその派生クラスは共通
namespace Kucl.Xml.XmlCfg {


    #region XmlConfigModel
    /// <summary>
    /// XmlContentsModelをベースに、Configクラス群として再定義されたクラスを表します。
    /// 基本動作はXmlContentsModelに従います。
    /// </summary>
    public class XmlConfigModel : XmlContentsModel {

        /// <summary>
        /// XmlConfigModelクラスの新しいインスタンスを初期化します。
        /// </summary>
        public XmlConfigModel()
            : base() {
        }
        /// <summary>
        /// XmlConfigPackageクラスのインスタンスを生成します。
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        protected override XmlContentsPackage CreateXmlContentsPackage(string packageName) {
            return new XmlConfigPackage(packageName);
        }
    }

    #endregion

    #region XmlConfigPackage
    /// <summary>
    /// XmlContentsPackageをベースに、Configクラス群として再定義されたクラスを表します。
    /// 基本動作はXmlContentsPackageに従います。
    /// </summary>
    public class XmlConfigPackage : XmlContentsPackage {
        /// <summary>
        /// 名前を指定して、XmlConfigPackageクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public XmlConfigPackage(string name)
            : base(name) {
        }
        /// <summary>
        /// XmlConfigPackageクラスの新しいインスタンスを初期化します。
        /// </summary>
        public XmlConfigPackage()
            : base() {
        }
        /// <summary>
        /// XmlConfigPackageをシリアル化する際のルートエレメント名を取得
        /// </summary>
        protected override string PackageRootElement {
            get {
                return "ConfigPackage";
            }
        }
        /// <summary>
        /// XmlConfigSectionクラスのインスタンスを生成します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override XmlContents CreateXmlContents(string name) {
            return new XmlConfigSection(name);
        }
    }
    #endregion

    #region XmlConfigSection
    /// <summary>
    /// XmlContentsをベースに、Configクラス群として再定義されたクラスを表します。
    /// 基本動作はXmlContentsに従います。
    /// </summary>
    public class XmlConfigSection : XmlContents {
        /// <summary>
        /// 名前を指定して、XmlConfigSectionクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public XmlConfigSection(string name)
            : base(name) {
        }
        /// <summary>
        /// XmlConfigSectionクラスの新しいインスタンスを初期化します。
        /// </summary>
        public XmlConfigSection()
            : base() {
        }
        /// <summary>
        /// XmlConfigSectionをシリアル化する際のルートエレメント名を取得します。
        /// </summary>
        protected override string ContentsRootElement {
            get {
                return "ConfigSection";
            }
        }
        /// <summary>
        /// XmlConfigItemProviderクラスのインスタンスを生成します。
        /// </summary>
        /// <returns></returns>
        protected override XmlContentsItemProvider CreateXmlContentsItemProvider() {
            return new XmlConfigItemProvider();
        }
    }
    #endregion

    #region XmlConfigItemProvider
    /// <summary>
    /// XmlContentsItemProviderをベースに、Configクラス群として再定義されたクラスを表します。
    /// 基本動作はXmlContentsItemProviderに従います。
    /// </summary>
    public class XmlConfigItemProvider : XmlContentsItemProvider {
        /// <summary>
        /// XmlConfigItemProviderクラスの新しいインスタンスを初期化します。
        /// </summary>
        public XmlConfigItemProvider()
            : base() {
        }
        /// <summary>
        /// XmlContentsItemをシリアル化する際のルートエレメント名を取得します。
        /// この文字列はXmlConfigとして使用される場合に適用されます。
        /// </summary>
        protected override string ItemRootElement {
            get {
                return "ConfigItem";
            }
        }
    }
    #endregion




    #region IUseConfig
    /// <summary>
    /// Configを使用するオブジェクトを示すインターフェースです。
    /// </summary>
    public interface IUseConfig {
        /// <summary>
        /// 既定値を示すConfigを作成します。
        /// </summary>
        /// <returns></returns>
        XmlConfigModel CreateDefaultConfig();
        /// <summary>
        /// configを適用します
        /// </summary>
        /// <param name="config"></param>
        void ApplyConfig(XmlConfigModel config);
        /// <summary>
        /// configを更新します。
        /// </summary>
        /// <param name="config"></param>
        void ReflectConfig(XmlConfigModel config);
        //string PackageName{get;}
        //string ConfigName{get;}
        /// <summary>
        /// 指定したConfig値が既定値かどうかを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsDefaultValue(string name, XmlContentsItem value);
    }
    #endregion

    #region UseConfigObjectCollection
    /// <summary>
    /// IUseConfigインターフェースを実装したオブジェクトのコレクションを表します。
    /// </summary>
    public class UseConfigObjectCollection : System.Collections.CollectionBase {
        /// <summary>
        /// IUseConfigオブジェクトを取得、設定するインデクサです。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IUseConfig this[int index] {
            get {
                return ((IUseConfig)(this.List[index]));
            }
            set {
                this.List[index] = value;
            }
        }
        /// <summary>
        /// IUseConfigオブジェクトをコレクションに追加します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Add(IUseConfig value) {
            return this.List.Add(value);
        }
        /// <summary>
        /// IUseConfigオブジェクトをコレクションから削除します。
        /// </summary>
        /// <param name="value"></param>
        public void Remove(IUseConfig value) {
            this.List.Remove(value);
        }
        /// <summary>
        /// 位置を指定して、IUseConfigオブジェクトをコレクションに挿入します。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Insert(int index, IUseConfig value) {
            this.List.Insert(index, value);
        }
        /// <summary>
        /// IUseConfigオブジェクトがコレクションに含まれているかどうかを返します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(IUseConfig value) {
            return this.List.Contains(value);
        }
        /// <summary>
        /// 指定したIUseConfigオブジェクトのコレクション内の位置を返します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(IUseConfig value) {
            return this.List.IndexOf(value);
        }
    }
    #endregion

    #region UseConfigHelper
    /// <summary>
    /// IUseConfigを実装するオブジェクトに対してのヘルパメソッドを提供するクラスです。
    /// </summary>
    public class UseConfigHelper {

        #region メンバ変数
        private XmlConfigModel m_DefaultConfig;
        private XmlConfigModel m_Config;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// UseConfigHelperオブジェクトの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="defaultConfig"></param>
        public UseConfigHelper(XmlConfigModel defaultConfig) {
            this.m_DefaultConfig = defaultConfig;
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// 対象となるConfigsオブジェクトを取得または設定します。
        /// </summary>
        public XmlConfigModel Config {
            get {
                return this.m_Config;
            }
            set {
                this.m_Config = value;
            }
        }
        #endregion

        #region IsDefaultValue
        /// <summary>
        /// 名前を指定してXmlContentsItemが変更されていないかどうかを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsDefaultValue(string name, XmlContentsItem value) {
            if (this.m_DefaultConfig.ContainsXmlContentsItem(name)) {
                XmlContentsItem item = this.m_DefaultConfig.GetXmlContentsItem(name);
                if (item != null && item.Type != XmlContentsItemType.Container) {
                    return item.Value.Equals(value.Value);
                }
            }
            return false;
        }
        #endregion

        #region GetXmlContentsItem
        /// <summary>
        /// 指定した名前を持つXmlContentsItemを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XmlContentsItem GetXmlContentsItem(string name) {
            if (this.m_Config.ContainsXmlContentsItem(name)) {
                return this.m_Config.GetXmlContentsItem(name);
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine("存在しないConfig\r\n" + name);
#endif
            //存在しない設定の場合、デフォルトの設定から取得
            if (this.m_DefaultConfig.ContainsXmlContentsItem(name)) {
                return this.m_DefaultConfig.GetXmlContentsItem(name);
            }
            //デフォルトにも存在しない場合は例外をスロー
            throw new ArgumentException(string.Format("{0}は存在しません", name));
        }
        #endregion

        #region 特定の型の値の取得

        #region GetIntFromConfig
        /// <summary>
        /// 指定した名前を持つ整数値を返します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetIntValue(string name) {
            return (int)this.GetXmlContentsItem(name).Value;
        }

        /// <summary>
        /// 指定した名前を持つ整数値を返します。
        /// 存在しない名前を指定した場合、defaultValueを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int GetIntValue(string name, int defaultValue) {
            try {
                return this.GetIntValue(name);
            }
            catch (ArgumentException) {
                return defaultValue;
            }
        }
        #endregion

        #region GetDoubleFromConfig
        /// <summary>
        /// 指定した名前を持つ実数値を取得します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public double GetDoubleValue(string name) {
            return (double)this.GetXmlContentsItem(name).Value;
        }

        /// <summary>
        /// 指定した名前を持つ実数値を取得します。
        /// 存在しない名前を指定した場合、defaultValueを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public double GetDoubleValue(string name, double defaultValue) {
            try {
                return this.GetDoubleValue(name);
            }
            catch (ArgumentException) {
                return defaultValue;
            }
        }

        #endregion

        #region GetStringFromConfig
        /// <summary>
        /// 指定した名前を持つ文字列を取得します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetStringValue(string name) {
            return (string)this.GetXmlContentsItem(name).Value;
        }

        /// <summary>
        /// 指定した名前を持つ文字列を取得します。
        /// 存在しない名前を指定した場合、defaultValueを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetStringValue(string name, string defaultValue) {
            try {
                return this.GetStringValue(name);
            }
            catch (ArgumentException) {
                return defaultValue;
            }
        }

        #endregion

        #region GetBoolFromConfig
        /// <summary>
        /// 指定した名前を持つbool値を取得します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetBoolValue(string name) {
            return (bool)this.GetXmlContentsItem(name).Value;
        }

        /// <summary>
        /// 指定した名前を持つbool値を取得します。
        /// 存在しない名前を指定した場合、defaultValueを返します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool GetBoolValue(string name, bool defaultValue) {
            try {
                return this.GetBoolValue(name);
            }
            catch (ArgumentException) {
                return defaultValue;
            }
        }

        #endregion

        #endregion

    }
    #endregion
}
