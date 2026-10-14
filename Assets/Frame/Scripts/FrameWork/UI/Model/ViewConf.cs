namespace FrameWork.UI
{
    public class ViewConf
    {
        public string Package;
        public string Path;
        public int id;

        public ViewConf(int id, string path)
        {
            Path = path;
            this.id = id;
        }

        public ViewConf(int id, string pack, string path)
        {
            Package = pack;
            Path = path;
            this.id = id;
        }
    }
}
