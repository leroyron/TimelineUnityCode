﻿partial class TIMELINE
{
    public partial class SCENES
    {
        public partial class DEMO2 : SCENE
        {
            public new int Actions()
            {
                action[200] = () =>
                {
                    timeline.Log("This is frame 200");
                    return 0;
                };

                action[500] = () =>
                {
                    timeline.Log("This is frame 500");
                    return 0;
                };

                return 0;
            }
        }
    }
}