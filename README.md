# StreaMe
Self Hosted Streaming Service

This application utilizes some of the well-know software architectures and specifications to implement a fully working streaming service (with some additional features!)

The goal was to build a self hosted streaming service that is capable of self monitoring, where data is collected from its usage, and can be inspected. All the data is collected, processed and stored locally for querying. This was done as learning exercise, and to develop a base starting point for monitoring .NET applications with the Open Telemetry specification and Grafana Stack.

## About Open Telemetry

Open Telemetry, or OTEL for short, is a industry standard set of "good practices" for generating and distributing application instrumentation data. Lets explore a few concepts of what OTEL gives us:

Telemetry, in software development, means collecting data about application usage. This data can be anything, ranging from simple strings that are emitted by your application as a means of inspecting what is going on, to correlated structured data with more meaningful interpretations. OTEL categorizes Telemetry intro 3 types of data, that are broad enough to fully qualify all the diagnostic information needed:

### Logs 
Any information that is emitted from your application. Normally, applications will log events from the different process that are executed, as in "Application started", or "Processing file X". OTEL goes beyond typical logs, and specify them as "structured logs", where the log message, and its variables contents, are separated. This allows for applications that consume/store logs to better index them, enabling users to query logs efficiently. 

Take a look at the sample (no specific language here): Log("Processing file {file}", "hello.txt"). Here we see three distinguished pieces that form the structured log:
1. The Body "Processing file {file}"
2. The Properties: file: "hello.txt"
3. The Message: Processing file hello.txt

Using this separated format, programs can query for specific properties on any log with said body, logs with the same "shape" based on the body, or the specific message. How this is used is dependent on the consumer of the logs, OTEL only specifies how to format said structured logs.

Also note that OTEL lets use use unstructured logs, but this is not recommended, and any application using this will lose precious information! See more on: https://opentelemetry.io/docs/concepts/signals/logs/

### Metrics
Logs can be useful for getting information on what is going on at some points, and totally can be used for generating application measurements. But its not its intended use. For this purpose, OTEL specifies "metrics", which are just measurements made at some point in time. 
Metrics are handle different by the OTEL specification, because they are optimized for a different end goal: To generate indicators of availability, performance, and usage. While logs can say what is going on, metrics can say how much of it is going on!
OTEL also specifies how those metrics can be collected, using "instruments". Said instruments are just standard measurement strategies that must be implemented by language. For instance, the "counter" is just a instrument that counts up, and never down.
And finishing off, OTEL also has the concept of "Aggregation" which lets us group metrics trough time, generating statistical values, and "Views" which lets us customize the output of said metrics/aggregations. See more one: https://opentelemetry.io/docs/concepts/signals/metrics/

### Traces
Logs by itself are useful for gathering information about your application. Traces however, expand on logs and lets your application group and "trace" calls inside your application, or across applications! While logs can say what happened in a single moment in time, a Trace represents a group of events across time, generating a timeline of events.

OTEL traces are composed of "spans", a collection of data that has a defined start and end timestamp. Spans also can be nested, or be adjacent to each other, much like a <div> html structure. This ensures a relationship between information flow, and allows users to visualize what is going on through time in the application.

Spans also can be linked to Logs, or can emit "Span Events". While logs can contain rich information with a lot of metadata associated to it, Span Events help developers to see marked occurrences. For example, in a multi step process, a Span can show when it starts and ends, a Span Event can mark the beginning and end of each step, and Logs can contain information on what exactly was done and what parameters where used. Think of span events as markers that will indicate specific points in time inside a Span.  