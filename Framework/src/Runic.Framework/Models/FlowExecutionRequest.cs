namespace Runic.Framework.Models
{
    public class FlowExecutionRequest
    {
        public string FlowName { get; set; }
        public int Threads { get; set; }
        public int RampUpMinutes { get; set; }
        public int RampDownMinutes { get; set; }
        public int ExecutionLengthMinutes { get; set; }
        public Flow Flow { get; set; }
        /*
         *  "FlowName": "MainOrderFlow",
    "Threads": 800,
    "RampUpMinutes": 10,
    "RampDownMinutes": 10,
    "ExecutionLengthMinutes": 60,
    Steps : [
       {
         "Function": "Login"
         "Repeat": 800
       },
       {
         "Function": "Search",
         "DistributionPercentage": 50,
         "InputDatasource": "datastore.test.postcodes",
         "DatasourceMapping": 
           {
             "POSTCODES_OUTCODE": "outcode"
           }
       },
       {
         "Function": "OpenItem",
          "DistributionPercentage": 30
       },
      {
        "Function": "Order",
        "DistributionPercentage": 20
      }
    ]
         */
    }
}