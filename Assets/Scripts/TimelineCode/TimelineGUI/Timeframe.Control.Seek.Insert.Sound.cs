public partial class Timeline
{
    public partial class GUI
    {
        public partial class Timeframe
        {
            public partial class Control
            {
                public partial class Seek
                {
                    public partial class Insert 
                    {
                        public Sound sound = new Sound();
                        public class Sound : Elements
                        {
                            Timeline _timeline;
                            GUI _gui;
                            public void Init(Timeline timeline)
                            {
                                this._timeline = timeline;
                                this._gui = timeline.gui;

                                TimelineCode.Log("(" + this._timeline.name + ") : Init Timeframe Control Seek Insert Sound");
                            }
                        }
                    }
                }
            }
        }
    }
}