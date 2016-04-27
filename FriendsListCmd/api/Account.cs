using System.Xml.Linq;

namespace FriendsListCmd.api
{
    public class Account
    {
        public Character Character { get; set; }
        public string Name { get; set; }
        public Stats Stats { get; set; }

        public Account(XElement elem)
        {
            Character = new Character(elem.Element("Character"));
            Name = elem.Element("Name").Value;
            Stats = new Stats(elem.Element("Stats").Element("Stats"));
        }
    }
}