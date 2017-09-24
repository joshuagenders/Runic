using Runic.Agent.Framework.Models;

namespace Runic.Agent.Core.TaskManagement
{
    public class JourneyMediator
    {
        private readonly IPersonAttributeService _personAttributeService;
        private readonly IPopulationController _populationController;

        //private TaskWorker _taskWorker { get; set; }
        private JourneyTask _journeyTask { get; set; }
        private IPersonFactory _personFactory { get; set; }
        public JourneyMediator(
            IPersonFactory personFactory, 
            IPersonAttributeService personAttributeService, 
            IPopulationController populationController)
        {
            _personFactory = personFactory;
            _personAttributeService = personAttributeService;
            _populationController = populationController;
        }

        public void InitialiseTask(Journey journey)
        {
            _journeyTask = new JourneyTask(journey, _personFactory, _personAttributeService, _populationController)
            {
                FailCommand = new FailCommand(),
                ProgressCommand = new ProgressCommand(),
                SuccessCommand = new SuccessCommand()
            };
            //_taskWorker = new TaskWorker(journeyTask);
            //_taskWorker.Construct();
            _journeyTask.Run();
        }

        public string GetProgress()
        {
            _journeyTask.ShowProgress();
            return _journeyTask.GetMessage();
        }

        public void FailTask()
        {
            _journeyTask.Fail();
        }

        public void SucceedTask()
        {
            _journeyTask.Succeed();
        }
    }
}
