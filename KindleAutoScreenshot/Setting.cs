using System.Xml.Linq;
using System.IO;

namespace KindleAutoScreenshot
{
    class Setting
    {
        private const string SettingFileName = @"setting.xml";
        //設定ファイルが存在するか
        static public bool HasSettingFile
        {
            get
            {
                return File.Exists(SettingFileName);
            }
        }

        //設定ファイル新規作成
        static public void Create(string savepath)
        {
            var xml = new XDocument(
                new XDeclaration("1.0", "utf-8", null)
                , new XComment("Setting")
                , new XElement(
                    "common"
                     , new XElement(
                            "savePath"
                            , new XText(savepath)
                    )
                )
            );
            xml.Save(SettingFileName);
        }
        //設定ファイルを読みに行く
        static public string Load()
        {
            var xml = XDocument.Load(SettingFileName);
            return xml.Root.Element("savePath").Value;
        }
        //設定ファイルを編集する
        static public void Edit(string savepath)
        {
            var xml = XDocument.Load(SettingFileName);
            var element = xml.Root.Element("savePath");
            element.Value = savepath;
            xml.Save(SettingFileName);
        }

    }
}
