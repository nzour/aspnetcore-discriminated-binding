# Aspnet Core Discriminated Binding
Aspnet Core Discriminated Binding.


### Installation 

```console
$ dotnet add package DiscriminatedBinding
```

---

### Enable feature

```c#
services.AddMvc(options => options.AddDiscriminatedModelBindingProvider());

// or

services.AddControllers()
    .AddMvcOptions(options => options.AddDiscriminatedModelBindingProvider());
```

---

### Doc

`DiscriminatorAttribute` defines property to read discriminator value from (see example below).

`DiscriminatorCaseAttibute` defines which instance should be associated with discriminator value.

The only requirement is that at least one `DiscriminatorCaseAttibute` should be defined.

#### What if we define multiple attribute with same values?
If few `DiscriminatorAttribute` was defined, then:
- No runtime errors.
- The first found one would be used.

If `DiscriminatorCaseAttibute`:

Actually, we generate dictionary (`<discriminator value>` to `<instance type>`).

So, the duplicated value would just override previous one

#### Which binding source is supported ?
- Body
- Form
- Query params
- Header params

#### Runtime errors
- `NoDiscriminatorCasesProvidedException` - at least one attribute `DiscriminatorCaseAttibute` should present
- `CouldNotReadDiscriminatorException` - unable read value with specified discriminator's property
- `UnresolvableDiscriminatorCaseException` - could not resolve instance type for specified discriminator value from request
- `NoInputFormatterFoundException` - trying to read unsupported content type from body
- `CouldNotResolveBinderForTypeException` - if is not possible to determine model binder for inferred concrete type for specified discriminator's value

---

### Example

```c#
[Discriminator(property: "type")]
[DiscriminatorCase(when: "github", then: typeof(GithubRepository))]
[DiscriminatorCase(when: "gitlab", then: typeof(GitlabRepository))]
public interface IRepository // this also may be common class or abstract class
{
    // the 'type' property may not be defined at all
}

public class GithubRepository : IRepository
{
    public string GithubToken { get; set; }
}

public class GitlabRepository : IRepository
{
    public string GitlabLogin { get; set; }
    public string GitlabPass { get; set; }
}
```

Controller looks like

```c#
public class RepositoryController
{
    [HttpPost("repositories")]
    public void Create([FromBody] IRepository repository) // or we may use [FromQuery], or from other supported binding sources
    {
        // ...
    }
}
```

If we want create github repo, then json schema should be like
```json
{
  "type": "github",
  "githubToken": "secret-token"
}
```

for gitlab:
```json
{
  "type": "gitlab",
  "gitlabLogin": "secret-login",
  "gitlabPass": "secret-pass"
}
```

---

### Not supported / Implementation required
- Unit tests
- Functional tests
- Enum as discriminator
- Support .Net Core 3.1 and .Net 5
- Parsing from `Body` supports only if we use `System.Text.Json` or `Newtonsoft.Json` input formatters.

    For example, xml body parsing not supported yet.

- Parsing from `Body` may consume additional memory, because it copies http request body to memory stream.

    If we use `System.Text.Json` parser - we don't need copy entire body stream into memory stream, so it is good to be refactored.

- Property defined in `DiscriminatorAttribute` may be named in any case.
  
  Internally, library would try to convert it to camelCase, PascalCase and underscored. 
  
  So, if we want to extend it and try another naming strategy - we have to change source code, so it would be nice to refactor and make this feature more extendable. 

- If we want to support another binding source (Body, Query, Header, etc), we have to change source code, this is also should not be this way.

---

### Inspired by
- [Hibernate discriminator entity inheritance](https://www.baeldung.com/hibernate-inheritance#1-discriminator-values)
- [Symfony discriminator deserialization](https://symfony.com/doc/current/components/serializer.html#serializing-interfaces-and-abstract-classes)
- [See also Typescript discriminated union type](https://www.typescriptlang.org/docs/handbook/2/narrowing.html#discriminated-unions)
