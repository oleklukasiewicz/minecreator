using minecreator.api.Model;

namespace minecreator.api.Bases.Top.Casual
{
    public class SlimTopCasualBaseTexture : IBaseTexture
    {
        public OutfitType Type { get; set; }
        public List<OutfitStyle> Styles { get; set; } = new List<OutfitStyle>();
        public OutfitModel Model { get; set; }
        public string Texture { get; set; }

        public SlimTopCasualBaseTexture()
        {
            Model = OutfitModel.SLIM;
            Type = OutfitType.TOP;
            Texture = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAZtJREFUeJztmtFxwyAMhtVeB9AIHo1RGIERPBojsIH7hI9QwYERNj3/30sS0AksS4IgiAAAAAAAAAAAAPA2vrQVGmMOZhb7Qgi07/vQmNbaI4SgoouI6GdUgUQIgYjonKAx5tDUz8znGKN8q2j5x0zxgDwE4m+tt6alhzQM0OveknwtZ2zb9qetRZdzrik/DBsgjcc4ODMTM5Nz7pxgOvGSfEoqn3pQSS7V1cOUHCBNojaxXnlNkARHFUiu7b3/cOu4bufJq0VeWvJquqjTe1RzQC1hpQ/ovf+YqLRKpA+R96d9S+aAq0jJcDZT9gEkvImWtbskI2V/LaYZ4MpmpbZhim3558h4pG0AKUalfqk9T3bxu7VW/Q9byhI54Ml9wJRlMJK7dK/bau75AQAAAAAAAAAAACJTj5uuEO8XMPP04zBa5Ugs586j8SUNcCdLGuDOs8DHc0Ctxl8qrmjdD6KZhZFWSvcL0n6aWB1aMgTu5PUGeDwESoWVUp1QuhswwuMG6MkBeZ8Grw+B5Q0we0/weAhcKa6iaAqAGr9JnPzi/q/1yQAAAABJRU5ErkJggg==";
        }

    }
}
