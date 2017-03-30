# LoadFileAdapter
LoadFileAdapter is an adapter for converting or modifying [load files](http://www.edrm.net/glossary/load-file/). The class library supports importing multiple load file types into a single document collection. Both metadata load files and image base load files can be imported and overlayed. The imported document collection captures family relationships and linked files, in addition to metadata. The imported document collection can be modified using a versatile set of transformation options. The modified document collection can then be exported to the same load file type or converted to an entirely different type.

## Supported Load File Types
* [Concordance DAT](http://help.lexisnexis.com/litigation/ac/cn_ev/importing_delimited_text_files.htm)
* [IPRO LFP](http://litsupport.org/knowledge-base/what-is-an-lfp/)
* [Opticon OPT](https://help.lexisnexis.com/litigation/ac/cn_classic/load_image_files_ci.htm)

## Implementation Example

```c#
public class Program
  {
      static void Main(string[] args)
      {          
            string xml = File.ReadAllText(args[0]);
            Job job = Job.Deserialize(xml);
            Executor executor = new Executor();
            executor.Execute(job);          
      }        
  }
```

## License
This project is licensed under the MIT License - see the [LICENSE.TXT](LICENSE.TXT) file for details.
