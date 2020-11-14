namespace GameFramework.AI.GOAP
{
    public class Patient : GAgent
    {
        private void Start()
        {
            base.Start();

            var s1 = new Goal("isWaiting", 1, true);
            Goals.Add(s1, 3);
        }
    }
}
