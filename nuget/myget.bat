REM right click on project and choose Pack to pack

nuget push "c:\temp\modules\Transformalize.Provider.CsvHelper.0.8.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.CsvHelper.0.8.1-beta.symbols.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.CsvHelper.Autofac.0.8.1-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.CsvHelper.Autofac.0.8.1-beta.symbols.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json


