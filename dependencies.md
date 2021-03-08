Go to http://www.webgraphviz.com/ and paste the following:

```
digraph G {
  "Blazor" -> "API"
  "Blazor" -> "SessionStorage"
  "Blazor" -> "EntityFramework"
  "Blazor" -> "MySQL"
  "API" -> "Microsoft CachingMemory"
  "API" -> "AspNet Core"
  "API" -> "EntityFramework"
  "API" -> "MySQL"
  "API" -> "Swagger"
}
```
