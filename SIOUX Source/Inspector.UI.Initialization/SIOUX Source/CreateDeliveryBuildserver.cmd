@rem Packager deliveries buildserver
@echo off

set destination=C:\Work\Inspector_Deliveries\InspectorDelivery
set xmldir=%destination%\XML
set xsddir=%destination%\XSD

IF exist %destination% (rmdir /S /Q %destination%)
mkdir %destination% && echo Directory %destination% created
mkdir %xmldir% && echo Directory %xmldir% created
mkdir %xsddir% && echo Directory %xsddir% created

xcopy Inspector.BusinessLogic.Data.Configuration\bin\Release\InspectionManager\Data\InspectionProcedure.xsd %xsddir% /Y
xcopy Inspector.BusinessLogic.Data.Configuration\bin\Release\InspectionManager\Data\InspectionStatus.xsd %xsddir% /Y
xcopy Inspector.BusinessLogic.Data.Configuration\bin\Release\InspectionManager\Data\PLEXOR.xsd %xsddir% /Y
xcopy Inspector.BusinessLogic.Data.Configuration\bin\Release\InspectionManager\Data\StationInformation.xsd %xsddir% /Y
xcopy Inspector.BusinessLogic.Data.Reporting\bin\Release\InspectionResultsData.xsd %xsddir% /Y

xcopy Inspector.BusinessLogic.Data.Configuration\bin\Release\InspectionManager\Data\InspectionStatus.xml %xmldir% /Y

xcopy Inspector.BusinessLogic.Data.Configuration\bin\Release\Inspector.BusinessLogic.Data.Configuration.dll %destination% /Y
xcopy Inspector.BusinessLogic.Data.Configuration.Interfaces\bin\Release\Inspector.BusinessLogic.Data.Configuration.Interfaces.dll %destination% /Y

xcopy Inspector.BusinessLogic.Data.Reporting\bin\Release\Inspector.BusinessLogic.Data.Reporting.dll %destination% /Y
xcopy Inspector.BusinessLogic.Data.Reporting.Interfaces\bin\Release\Inspector.BusinessLogic.Data.Reporting.Interfaces.dll %destination% /Y

xcopy Inspector.BusinessLogic\bin\Release\Inspector.BusinessLogic.dll %destination% /Y
xcopy Inspector.BusinessLogic.Interfaces\bin\Release\Inspector.BusinessLogic.Interfaces.dll %destination% /Y
xcopy Inspector.BusinessLogic\bin\Release\KAM.Inspector.Infra.dll %destination% /Y

xcopy Inspector.Connection.Manager\bin\Release\Inspector.Connection.Manager.dll %destination% /Y
xcopy Inspector.Connection.Manager.Interfaces\bin\Release\Inspector.Connection.Manager.Interfaces.dll %destination% /Y

xcopy Inspector.Connection.StateMachine\bin\Release\Inspector.Connection.StateMachine.dll %destination% /Y

xcopy Inspector.Hal\bin\Release\Inspector.Hal.dll %destination% /Y
xcopy Inspector.Hal.Infra\bin\Release\Inspector.Hal.Infra.dll %destination% /Y
xcopy Inspector.Hal.Interfaces\bin\Release\Inspector.Hal.Interfaces.dll %destination% /Y
xcopy Inspector.Hal\bin\Release\msvcp100.dll %destination% /Y
xcopy Inspector.Hal\bin\Release\msvcr100.dll %destination% /Y
xcopy Inspector.Hal\bin\Release\wcl.dll %destination% /Y

xcopy Inspector.Infra\bin\Release\Inspector.Infra.dll %destination% /Y

xcopy Inspector.Model\bin\Release\Inspector.Model.dll %destination% /Y

xcopy Inspector.UI\bin\Release\INSPECTORSettings.xml %destination% /Y
xcopy Inspector.UI\bin\Release\Spring.Core.dll %destination% /Y
xcopy Inspector.UI\bin\Release\Common.Logging.dll %destination% /Y
xcopy Inspector.UI\bin\Release\log4net.dll %destination% /Y

echo.
echo ==========================================================
echo Deliveries collected in network share
echo ==========================================================
echo.