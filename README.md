# SimplySqlSchema

Some crappy little abstractions over SQL, for creating, migrating and querying tables.

[Available On Nuget](https://www.nuget.org/packages/SimplySqlSchema/)

## Useful Features

- Schema Extraction From DotNet Objects ([IObjectSchemaSchemaExtractor.cs](SimplySqlSchema\Extractor\IObjectSchemaExtractor.cs))
- Table Creation from Schemas ([ISchemaManager.cs](SimplySqlSchema\Manager\ISchemaManager.cs))
- Schema Migration between Releases ([ISchemaMigrator.cs](SimplySqlSchema\Migration\ISchemaMigrator.cs))
- Support for SQLite, SqlServer and MySQL ([ISchemaManagerDelegator.cs](SimplySqlSchema\Delegator\ISchemaManagerDelegator.cs))
- Basic CRUD Support for Schemas ([ISchemaQuerier.cs](SimplySqlSchema\Query\ISchemaQuerier.cs))


## Example Usage

#### Introductury Usage
The example accomplishes the following tasks in order:

1. Determine the table schema from a POCO.
2. Apply that schema to the DB.
3. Do a Insert and Get on the table.

```csharp
class TestObject
{
  [Key]
  public int Key { get; set; }

  [MaxLength(100)]
  public string Value { get; set; }
}

public async Task Demo()
{
  // 1. Determine the table schema from the POCO
  IObjectSchemaExtractor extractor = new DataAnnotationsSchemaExtractor();
  ObjectSchema schema = extractor.GetObjectSchema(typeof(TestObject), "TestTable");

  // 2. Apply the schema to the DB
  IDbConnection connection = new SQLiteConnection($"Data Source=Test.db;Version=3;");
  ISchemaManager manager = new SQLiteSchemaManager();
  await manager.CreateObject(connection, schema);

  // 3. Insert and Get
  ISchemaQuerier querier = new SQLiteQuerier();
  await querier.Insert(connection, schema, obj: new TestObject()
  {
      Key = 10,
      Value = "Hey there"
  });
  TestObject fetched = await querier.Get(connection, schema, keyedObject: new TestObject()
  {
      Key = 10
  });
}
```

#### Registering with .NET core

The following example adds all the required interfaces to the .NET core dependency container.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSimplySqlSchema();
}
```

#### Backend-Agnostic Example

The `ISchemaManagerDelegator` abstracts away the backend providers, so that backends can 
be determined at runtime, or multiple different backends can be used within the same project. This example gets a schema from the object, and safely migrates it, assuming
the POCO was different last time the code was run (but works if nothing has changed as well).

```csharp
// This method assumes dependency injection for simplicity
public async Task Demo(ISchemaManagerDelegator delegator, ISchemaMigrator migrator, IDbConnection connection)
{
    var target = delegator.GetObjectSchemaExtractor().GetObjectSchema(typeof(TestObject));
    var manager = delegator.GetSchemaManager(BackendType.MySql);

    await migrator.PlanAndExecute(
      connection: connection,
      targetManager: manager,
      targetSchema: target,
      options: new MigrationOptions()
      {
        AllowDataloss = false,
        ForceDropRecreate = false
      }
    );
}
```


## Built With

* [Dapper](https://www.nuget.org/packages/Dapper) - For querying and parameterization
