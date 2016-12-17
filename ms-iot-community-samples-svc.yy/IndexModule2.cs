namespace ms_iot_community_samples_svc
{
    using Nancy;

    public class IndexModule2 : NancyModule
    {
        public IndexModule2()
        {
            Get["/"] = parameters =>
            {
                return View["index2"];
            };
        }
    }
}