namespace FlexiTools.Model
{
    internal class SideMenu
    {
        public string Name { get; set; }
        public bool IsExpanded { get; set; }
        public List<SideMenu> SubMenus { get; set; } = new List<SideMenu>();
    }
}
