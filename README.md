![Runic](images/runic_logo_2.png)

Runic is a test lab and framework for running distributed automated tests. The tool is designed to support functional and performance tests.
## Concepts
### Runes
Quite often when test against a system we want to generate load in particular parts of an application, but also want to execute functional paths.
This can be difficult when each component of the system under test requires state or data from other parts of the system. 
Normally, a functional flow will move through each part of the application; control of the ratio of load applied to the system becomes more difficult in this model.
Runes are designed to help mitigate this issue, by promoting modular functions which can share data. A rune is the result of an action executed against the SUT.
The rune is stored and can be re-used later by other functions. In this model you could for example re-use logins and control the exact number of users dynamically.

This model flips test architecture on its side. The model is best suited to architectures where transactional state is flexible or minimal such as microservice architecture.
Using this framework you have complete control over the amount of load on each invdividual component of the system, but you can also simulate functional flows through the system by capturing and reusing state obtained by performing actions in the system under test.

## Goals
What I want to achieve:
 - Make it easier
   - Build out a polished framework where the engineer can not only create tests efficiently, but also use the framework to perform exploratory testing.
 - Fine grained control
   - Create, alter and schedule tests real-time
   - Control test data feeds real-time
 - A synthetic user generation framework
   - Able to simulate real system interactions
   - Promote modular and flexible test design to enhance code re-use, maintainability
 - Is highly-scalable
   - Dockerized and performant
 - Support Continuous Testing
   - Enabling real-time feedback loops for functional and non-functional attributes
 - Flexible
   - Tests written in .NET core, meaning tests have the control to exercise the SUT at many levels
 
Principles:
  - Continuous testing
  - Modular test design
  - Data driven testing
  - Robust and flexible tests

## The framework
The Runic framework is designed around user interactions. It supports a data-driven model of testing. 
Performing functions in the system produces data, and potentially back-end state, which can be surfaced and used in subsequent functions.

The framework designs tests that execute series of functions, and produce and store information as "runes". Some functions will require runes from other functions, and can retrieve them from the rune database. This way tests can be constructed to re-use data.  Each time a runic function finds information that might be useful to another function, or to analyse for functional acceptance, it stores it as a rune.

Functions can span across multiple pages, or APIs etc. to achieve its purpose. Some functions may also incorporate sub-functions, such as constructing a cart for an order function. The idea is to create re-usable functions that can be stringed together dynamically.

When constructing a framework, careful thought should be given to how to break up functions and how to standardise runes. Functions should be constructed to enable the most control over their actions, but also ease of integration by well-designed dependencies. Data that cannot be sourced from runes can be passed into the tests from the test data store. The test should be able to function without this input wherever possible.

### Attributes
BeforeEach - Decorates a method to be exected before each function in the class
AfterEach - Decorates a method to be exected after each function in the class
ClassInitialise - Decorates a method to be exected upon class instantiation
ClassTeardown - Decorates a method to be exected upon object disposal
Function - Decorates a method as a function. A mandatory parameter 'FunctionName' is used to identify the function.

#### TODO Attributes
MinesRune, Requires, RequiresLinkedRunes - TODO will be used to provide metadata about what runes a function creates and requires

### Standard structure of a function:

![Runic Functions](images/FunctionDesign.png)


## Runner Concepts
### Functions
A function will maintain state throughout it's execution. Be careful to manage memory wisely. 
This decision was made to provide the most control over execution to the test as possible.
Functions are used as steps as part of a flow.
 
### Flows
Flows are groups of functions which can be executed by a Runic agent. A flow could be a single repeating function or a complex series of functions.
By default steps are executed in order, but an additional flag can be set to jump to a new step by step name, returned as a string from the function itself.
Thread patterns are implemented as scheduled updates to the thread level of flows.

### Thread Patterns
There are three supported types of thread pattern:
 * Constant Thread Patterns - Maintains a consistent thread level throughout the test
 * Gradual Thread Patterns - Thread pattern allows for an optional ramp up and ramp down in thread level
 * Graph Thread Patterns - Thread pattern that allows mapped points of thread levels, allowing thread levels to follow any pattern desired

## Implementing the runner
### Key Interfaces to implement
There are many interfaces to implement, and different consequences of implementation design. Some key interfaces are their purpose are detailed below.

#### IStats
The IStats interface is used to push stats about test execution to a time-series database like graphite or through a proxy like Statsd.
Implementations using the StatsN and the JustEat Statsd packages are provided in the core library.
    public interface IStatsClient
    {
        void CountPluginLoaded();
        void CountFlowAdded(string flowName);
        void CountFunctionSuccess(string functionName);
        void CountFunctionFailure(string functionName);
        void SetThreadLevel(string flowName, int threadCount);
        void CountHttpRequestSuccess(string functionName, string responseCode);
        void CountHttpRequestFailure(string functionName, string responseCode);
        void Time(string functionName, string actionName, Action actionToTime);
    }

#### IRuneClient
The IRuneClient is used to store and query runes. Runes are queried by functions whenever state is required to perform that function.
The implementation of the rune client decides how runes will be stored, and how runes will be served to functions based on the query provided by the function.
Possible implementations are using queues or in memory storage.
Some factors to consider are:
 * TTL on runes (to avoid stale state)
 * Exclusivity of rune use (to avoid duplicate use of state)
 * Efficiency of the rune querying implementation (for performance)
```
    public interface IRuneClient
    {
        Task<RuneQuery> RetrieveRunes(RuneQuery query);
        Task SendRunes<T>(params T[] runes) where T : Rune;
    }
```

##### Note
The IStats and IRuneClient implementations are injected into the function assemblies wherever a public static property is found of their respective types.
 
#### IFlowManager
The flow manager is used to store and manage flow objects. The test agent invokes the flow manager whenever a new flow is requested to be started.
The flow manager could be implemented in memory or could use an external datasource like redis, dynamodb or even a web service.
```
    public interface IFlowManager
    {
        void AddUpdateFlow(Flow flow);
        Flow GetFlow(string name);
        IList<Flow> GetFlows();
    }
```

#### IDataService
The Data Service can be used to inject data into a function through communication with a datastore. This functionality can also be used to 'inject' data into a flow at runtime.
The IDataService takes a datasource ID and datasource mapping. The Datasource ID is used to identify a datastore such as a table name or queue name. 
The datasource mapping is a dictionary used to map the data from the datasource into the return object array. 
The return object array are the input parameters to a method (Runic function).
As a practice, all functions should have defaulted method parameters and handle cases where default inputs are used.
```
    public interface IDataService
    {
        object[] GetMethodParameterValues(string datasourceId, Dictionary<string, string> datasourceMapping);
    }
```

#### IPluginProvider
The IPluginProvider is used to retrieve function dlls. 
If the assembly dlls are already available to the plugin manager, then the RetrieveSourceDll method does not need to do anything. 
The GetFilepath should return the filepath of the function dll based on the key provided.
```
    public interface IPluginProvider
    {
        void RetrieveSourceDll(string key);
        string GetFilepath(string key);
    }
``` 
## The Agent 
The Agent is responsible for executing the functions or tests. The agent loads the required executables dynamically. The agent also reports all timing related statistics to graphite. The agent executes functions or tests based on messages received from a controller. Agents support multiple threads, however a test dll will only be loaded once.


# Future features
#### Possible feature: Data reservation
The data reservation function can reserve an indexed field for use until a timeout period expires or until freed by the agent. As an example use case, any time an agent wants to use a customer id exclusively, it reserves the Id. The database rune query service can then exclude any runes for that customer id from use. This is not a completely safe solution but may be a handy tool.

#### Possible feature: Registering transforms
Runic functions can have data fed to them from various sources through a single input queue, allowing real-time control of test data.
Where an appropriate rune cannot be found for a test, if a transform can be found that converts an existing rune then it will be parsed.

#### Possible feature: Data visibility
It is common that applications will require data only to be available to a user under certain circumstances, for example, authenticated users. When storing runes I may add some implementation of visibility levels. This will likely also involve restricting runes to particular users, roles, or other abstract concepts.

## Test Data store
The test data store can be used to store data that tests need that cannot be sourced through runes.
The user can map tables, stored procedures or queries through to inputs for tests. The user can also created user-defined lists, tables and data-structures (like JSON) as inputs to tests. 

A possible function is to create an event system to synchronize changes to test data. The data store could subscribe to certain tests/functions or runes and update state based on the results. Although this approach does assume that the data surfaced on the front end; in the case of bugs the data may need to be sanitized. As a precaution, data could be set with expiry so only relatively new state is used for tests.
The other model is to regularly update the data store with state from the target system’s database.

## Runic UI
Runic UI provides user access to the test lab and oracle functionality. The UI can be used to design and construct workflows, create and execute test plans and manage test data. The oracle is used to analyse the test results in depth and perform functional checks. 
Angular2 or React are likely choices for the UI. 

![Runic UI](images/ExampleTestComposer.png)

Some types of verification to support in future are:
 * Expressing predicates to source results to verify back-end state
 * Expressing predicates to source results to verify front end funtionality or state
 * Locating outliers in function and flow results in comparison to comparable result sets and averages
 * General statisticical analytics of performance characteristics
 
I will introduce more templated analytics at a later stage, that can also utilise the performance outputs from the framework.


## The Oracle
The oracle is (most likely a group of services) responsible for analysing the results from tests executed in a Runic test lab. The oracle should be designed so that the user can express the pre-conditions and expected outcomes for a test. The test can then be executed against any runes which match the pre-conditions. There may be a need to support several methods of expression, including external script execution. Possibly the use of hamcrest matchers.


## Kamon
For Graphana functionality.
