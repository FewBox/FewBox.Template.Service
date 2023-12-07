namespace FewBox.Template.Service.Model.Entities
{
    public partial class Feedback
    {
        public FeedbackLevel Level
        {
            get
            {
                if (this.Score > 3)
                {
                    return FeedbackLevel.Positive;
                }
                else
                {
                    return FeedbackLevel.Negative;
                }
            }
        }
    }
}