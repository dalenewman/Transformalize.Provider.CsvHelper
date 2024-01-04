nuget pack Transformalize.Provider.CsvHelper.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Provider.CsvHelper.Autofac.nuspec -OutputDirectory "c:\temp\modules"

nuget push "c:\temp\modules\Transformalize.Provider.CsvHelper.0.10.2-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.CsvHelper.Autofac.0.10.2-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json


