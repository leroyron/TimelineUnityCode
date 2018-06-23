﻿public partial class TIMELINE
{
    public partial class SCENES
    {
        public partial class MAIN : SCENE
        {
            public override int Actions()
            {
                actions[200] = () =>
                {
                    TIMELINE.Log("This is frame 200");
                    return 0;
                };

                actions[500] = () =>
                {
                    TIMELINE.Log("This is frame 500");
                    return 0;
                };
                return 0;
            }
        }
    }
}