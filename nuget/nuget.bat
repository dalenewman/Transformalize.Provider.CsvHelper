nuget pack Transformalize.Provider.CsvHelper.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Provider.CsvHelper.Autofac.nuspec -OutputDirectory "c:\temp\modules"

REM nuget push "c:\temp\modules\Transformalize.Provider.CsvHelper.0.8.29-beta.nupkg" -source https://api.nuget.org/v3/index.json
REM nuget push "c:\temp\modules\Transformalize.Provider.CsvHelper.Autofac.0.8.29-beta.nupkg" -source https://api.nuget.org/v3/index.json


