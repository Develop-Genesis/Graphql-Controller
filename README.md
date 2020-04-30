# Graphql-Controller
A graphql library that integrates nice with .net 



# Inspiration
Graphql is the strong typed protocol to pass data, that is the way I see it so my goal
with this project is simple, use the strong typed system of .Net to create your Graphql schema 
in a way that you feel it natural.

# Getting Started with Asp.net core
##### Install nuget package:

` dotnet add package GraphqlController --version 1.1.0 `

` dotnet add package GraphqlController.AspNetCore --version 1.1.0 `

##### How to use it

Configure your services

```csharp

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Call this two methods
            services
                    // Add GraphQlController to your project
                    .AddGraphQlController()
                    // Use the current assembly to locate the graphql types 
                    .AddCurrentAssembly(); 
            
            // Call this to add http enpoint support 
            services.AddGraphQlEndpoint();
        }        
    
```

Create your graphql types using normal C#, 
make sure to create them in the same assembly

```csharp
   
    public class Student
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Grades { get; set; }
    }

    public class Teacher
    {
        public string Name { get; set; }
        public string LastName { get; set; }        
    }

```

Create your root type.
The root type is the top of the graph where you can access the data
and it also represent a graphql api

```csharp
   
   // Must have the attribute [RootType]  
   // and must derive from GraphNodeType
   [RootType]
   public class Root : GraphNodeType
   {
      public Student Student => new Student()
      { 
         Name => "Jhon",
         LastName => "Robinson",
         Grades => 80
      }

      public Teacher Teacher = new Teacher()
      {
         Name => "Alejandro",
         LastName => "Guardiola"
      }
   }

```

As last step we have to declare what the path for this api should be,
in our Startup.cs class:

```csharp
  app.UseEndpoints(endpoints =>
  {     
     // Include this line to declare that the root with type Root
     // will be serving at /graphql 
     endpoints.MapGraphQLEnpoint<Root>("/graphql");                
     endpoints.MapControllers();
  });
```

If you run the project and call the api at /graphql using any tool like
graphiql you can query this schema like:

```graphql
   
   {
      student
      {
        name
        lastName
        grades
      }
      
      teacher
      {
        name
        lastName        
      }
      
   }

```

and get as result

```json

   {
      "student": 
      {
         "name": "Jhon",
         "lastName": "Robinson",
         "grades": 80
      },
      "teacher":
      {
         "name": "Alejandro",
         "lastName": "Guardiola",
      }
   }

```

##### Attributes

To include attributes instead of using a property use a function

Root.cs
```csharp
   
    public Student Student(string studentId)
    { 
       return new Student()
       {
          Name => "Jhon",
          LastName => "Robinson",
          Grades => 80
       }       
    }

```

graphql:
```graphql
    
    {
       student(studentId: "somerandomid")
       {
         name
       }
    }

```
```json

    {
        "student": 
        {
           "name": "Jhon"
        }
    }    

```

##### Complex Attributes
To create custom attribute types, just create the .net class for it
```csharp
   
    public Student Student(SearchCriteria search)
    { 
       return new Student()
       {
          Name => "Jhon",
          LastName => "Robinson",
          Grades => 80
       }       
    }
    
    // All properties must have both setters
    public class SearchCriteria
    {
        public string Name { get; set; }
        public string LastName { get; se; }
    }

```
```graphql
    
    {
       student(search: { name: "some name", lastName: "some last name" })
       {
         name
       }
    }

```
```json

    {
        "student": 
        {
           "name": "Jhon"
        }
    }    

```
##### Lists

Every type that derive from IEnumerable will be considered as a list,
for example we can add a new method in our Root to get all the students

Root.cs:
```csharp
    
   // this method is inside the Root class
    public IEnumerable<Student> AllStudents(int skip, int take)
    {
        return new List<Student>()
        {
            new Student()
            {
                Name => "Jhon",
                LastName => "Robinson",
                Grades => 80
            },
            new Student()
            {
                Name => "Jhonatan",
                LastName => "Cruz",
                Grades => 60
            },
            new Student()
            {
                Name => "Rubio",
                LastName => "Acosta",
                Grades => 30
            },
        }.Skip(skip).Take(take);
    }

```
```graphql
    
    {
       allStudent(skip: 1, take: 2)
       {
         name
       }
    }

```
```json

    {
        "allStudent": [
           {
              "name": "Jhonatan"
           },
           {
              "name": "Rubio"
           }
        ]        
    }    

```

##### Non null types
To mark a type as non null you have to use the [NonNull] attribute,
we can put the attribute in anything we want to be non null
for example:
```csharp
    
    // this method is inside the Root class

    // the return type has be marked as non null
    [NonNull]
                                           // the attributes are marked as non null as well
    public IEnumerable<Student> AllStudents([NonNull]int skip, [NonNull]int take)
    {
        return new List<Student>()
        {
            new Student()
            {
                Name => "Jhon",
                LastName => "Robinson",
                Grades => 80
            },
            new Student()
            {
                Name => "Jhonatan",
                LastName => "Cruz",
                Grades => 60
            },
            new Student()
            {
                Name => "Rubio",
                LastName => "Acosta",
                Grades => 30
            },
        }.Skip(skip).Take(take);
    }

```

##### Interfaces
To use interfaces you just have to do it as you normally do in .net,
for example lets create an interface for Student and Teacher sharing name and lastName:

```csharp
   
    public interface IPerson
    {
        public string Name { get; set; }
        public string LastName { get; set; }
    }

    public class Student : IPerson
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Grades { get; set; }
    }

    public class Teacher : IPerson
    {
        public string Name { get; set; }
        public string LastName { get; set; }        
    }

```

now we can add a field in the root type to return the interface

Root.cs
```csharp
    
    // this method is inside the Root class
    public IPerson Person(string personId)
    {
        return Teacher = new Teacher()
        {
            Name => "Alejandro",
            LastName => "Guardiola"
        }
    }

```
graphql:
```graphql
    
    {
       person(personId: "somerandomId")
       {
           name
           lastName
           ... on Student
           {
              grades
           }
       }
    }

```
```json

    {
        "person": 
        {
           "name": "Alejandro",
           "lastName": "Guardiola"
        }        
    }    

```

##### Unions
For union types use the Union<> class or derive from it,
for example if we want to create an union of string and int
we return Union<string, int> or create a class that derive from it.

Root.cs
```csharp
    
    // this property is inside the Root class
    public Union<string, int> UnionTest => new Union<string, int>("My value");    
    // this property is inside the Root class
    public Union<string, int> UnionTest2 => new Union<string, int>(10);

```

graphql:
```graphql
    
    {
       unionTest
       unionTest2
    }

```
```json

    {
        "unionTest": "My value",
        "unionTest2": 10              
    }    

```

##### Mutations

To create mutations just create a class or multiple classes
with the mutations but it should derive from GraphNodeType and have the mutation attribute 
with the Root type that they are mutating

```csharp
   
   // must have this attribute with the type of the root that they mutate
   [Mutation(tyepof(Root))]
   public class StudentMutations : GraphNodeType
   {
      // inside the class we create the functions for the mutations
      public Student AddStudent([NonNull]StudentInput student)
      {
          // logic to add student to the database
          return new Student()
          {
             Name = student.Name,
             LastName = student.LastName,
             Grades = student.Grades
          }
      }
   }
   
   public class StudentInput
   {
      [NonNull]
      public string Name { get; set; }
      [NonNull]
      public string LastName { get; set; }
      [NonNull]
      public int Grades { get; set; }
   }

```

graphql:
```graphql
    
    mutation {
       addStudent(student:{
          name: "Alejandro",
          lastName: "Guardiola",
          grades: 0
       })
       {
          name
       }
    }

```
```json

    {
        "addStudent": 
        {
            "name": "Alejandro"
        }
    }    

```

#### Type description

Thre is two ways to create a description for a type

##### Using the Description Attribute

Putting the description attribute where we want the description

```csharp
   
    [Description("Represent a person")]
    public interface IPerson
    {
        [Description("Person name.")]
        public string Name { get; set; }
        [Description("Person last name.")]
        public string LastName { get; set; }
    }
    
    [Description("Represent an student")]
    public class Student : IPerson
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Grades { get; set; }
    }

```

the attribute can be used for the fields attributes as well

##### Using the C# xml documentation

Just creating a xml documentation for the property or method where we want the description

```csharp
   
   [Mutation(typeof(Root))]
    public class StudentMutations : GraphNodeType
    {
        /// <summary>
        /// Add a new student to the database
        /// </summary>
        /// <param name="student">The student to add</param>
        /// <returns></returns>
        public Student AddStudent([NonNull]StudentInput student)
        {
            
            return new Student()
            {
                Name = student.Name,
                LastName = student.LastName,
                Grades = student.Grades
            };
        }
    }

    public class StudentInput
    {
        /// <summary>
        /// Name of the student
        /// </summary>
        [NonNull]
        public string Name { get; set; }

        /// <summary>
        /// Last name of the student
        /// </summary>
        [NonNull]
        public string LastName { get; set; }

        /// <summary>
        /// Grades
        /// </summary>
        [NonNull]
        public int Grades { get; set; }
    }

```

The only downside of this method is that for this to work you have to include the
xml documentation file in the build

#### Custom type name
To asignate a custom name to the type, the type must have the Name attribute

```csharp
    
    [Name("MyCustomStudentTypeName")]
    public class Student : IPerson
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Grades { get; set; }
    }

```

this can be used for the attributes as well

#### Async - Await
When  you return a Task<> from a field GraphQLController automatically
take of that.

You can get the CancellationToken by adding it in the method parameters

```csharp
    public async Task<Student> AddStudent(StudentInput student, CancellationToken cancellationToken)
    {
       return await // some result      
    }
```

#### Dependency Injection
GraphqlController support Dependency Injection out of the box, it integrates 
with the built-in asp.net core dependency injection.
To inject services to a type it must derive from GraphNodeType, because the root
 and the mutations types already has to derive from it, you can inject services in the root and mutations
already.

Root.cs:
```csharp
    
   [RootType]
   public class Root : GraphNodeType
   {
        // private methods and properties are ignored
        private StudentRepository _studentRepo;

        public Root(StudentRepository studentRepo)
        {
            _studentRepo = studentRepo;
        }
        
        // exposed by the graphql api
        public List<Student> AllStudents => _studentRepo.GetAll();
   }

```

#### Dependency Injection in nested types
In order to use dependency injection in nested types, again they have
to derive from GraphNodeType and be created using IGraphqlResolver service.
Classes that derive from GraphNodeType can override the OnCreateAsync function
to initialize

Lets modify the Teacher class a little:

```csharp

      // TeacherDomain in the genric parameter
      // is a custom parameter that we can pass to the instance
      // to initialized
      public class Teacher : GraphNodeType<TeacherDomain>
      {        
        StudentRepo _studentRepo;

        // Inject services
        public Teacher(StudentRepo studentRepo)
        {
            _studentRepo = studentRepo;
        }        
        
        public override Task OnCreateAsync(TeacherDomain domain, CancellationToken cancellationToken)
        {
            // Initialize the instance here
            Name = domain.Name;
            LastName = domain.LastName;
            return Task.CompletedTask;
        }

        public string Name { get; set; }
        public string LastName { get; set; }    

        public IEnumerable<Student> Students()
        {
            return _studentRepo.GetTeacherStudents(Name);
        }  
        
    
      }
```

Now to create the teacher from another type we use the IGraphqlResolver service:

Root.cs
```csharp
    
    // services injected in Root
    IGraphqlResolver _graphqlResolver;
    TeacherRepo _teacherRepo;

    // this method is inside Root.cs
    public async Task<Teacher> Teacher(string name, CancellationToken cancellationToken)
    {
        var teacherDomain = _techerRepo.GetTeacherByName(name);
        // create teacher using IGraphqlResolver
        return await _graphqlResolver.CreateGraphqlEnityAsync<Teacher, TeacherDomain>(teacherDomain, cancellationToken);
    }

```

## Persisted Queries
A persisted query is an ID or hash that can be sent to the server instead of the entire GraphQL query string. This smaller signature reduces bandwidth utilization and speeds up client loading times. Persisted queries are especially nice paired with GET requests, enabling the browser cache and integration with a CDN.
(Copied from the Apollo documentation)

Persisted queries are implmented using the Apollo protocol
https://github.com/apollographql/apollo-link-persisted-queries#protocol

#### How to enable Persisted Queries

Persisted queries can be cached using memory cache or a distributed cache.

##### Persisted queries using Memory Cache
Using memory cache the queries are stored on the app memory. This aproach works 
well for the majority of many cases:

Pros:
   * It is fast because works with the app memory
   * Dont need an external service or store
   * Dont need any configuration

Cons:
   * Cannot be share between multiple app instances
   * The cache is reseted every time the app restart
   * Many queries cached can make your memory go higher

To enable Persisted queries with memory cache call, you have to make sure 
you have Asp.net core memory cache added to your services you can learn more here:
https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-3.1

In your Startup.cs in ConfigureServices add:
```csharp

    // add asp.net core memory cache
    services.AddMemoryCache();

    services.AddGraphQlEndpoint()
    // add in memory persisted query support
            .AddInMemoryPersistedQuery();

```

## Cache

To add cache to the resources that don't change frecuently you have to use the CacheAttribute
and specifing the max age in second that the resource will can be cached.

```csharp

```

##### Persisted queries using Distributed Cache
(From Microsoft docs)
A distributed cache is a cache shared by multiple app servers, typically maintained as an external service to the app servers that access it. A distributed cache can improve the performance and scalability of an ASP.NET Core app, especially when the app is hosted by a cloud service or a server farm.

A distributed cache has several advantages over other caching scenarios where cached data is stored on individual app servers.

Pros:
  * Is coherent (consistent) across requests to multiple servers.
  * Survives server restarts and app deployments.
  * Doesn't use local memory.

Cons:
  * Require external cache service (Redis, NCache, etc...)
  * Require more configuration
  * Slighty slower than Memory Cache

To use it first add the distributed cache: 
https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-3.1

and then call AddDistributedPersistedQuery in your services.

In your Startup.cs in ConfigureServices add:
```csharp

    // add asp.net core memory cache
    services.AddMemoryCache();

    services.AddGraphQlEndpoint()
    // add distributed persisted query support
            .AddDistributedPersistedQuery();
```

#### Progress
This library is still in development and has not be tested in production

##### Comming features
* Subscriptions
* Support for the graphql net server project
* Posibly more features to add and bugs to fix


#### Special Thanks
* This library is not possible without https://github.com/graphql-dotnet/graphql-dotnet

* And also to make the xml documentation descriptions https://github.com/loxsmoke/DocXml