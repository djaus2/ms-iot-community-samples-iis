namespace ms_iot_community_samples_svc
{
    using Nancy;

    public class IndexModule1 : NancyModule
    {
        public IndexModule1()
        {
            Get["/"] = parameters =>
            {
                return View["index1"];
            };
        }
    }
}