REM right click on project and choose Pack to pack

nuget push "c:\temp\modules\Transformalize.Provider.CsvHelper.0.7.6-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.CsvHelper.0.7.6-beta.symbols.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.CsvHelper.Autofac.0.7.6-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.CsvHelper.Autofac.0.7.6-beta.symbols.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json


