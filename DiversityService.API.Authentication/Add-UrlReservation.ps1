#################################################
# Registers an URL reservation for the local service
# and adds a mapping to the hosts file so the domain points to localhost
#################################################

$host = "local.diversityapi.azurewebsites.net"
$port = 5000
$endpointUrl = "http://${host}:${port}/"

#################################################
# First, make sure we are running elevated
#################################################

# Get the ID and security principal of the current user account
$myWindowsID=[System.Security.Principal.WindowsIdentity]::GetCurrent()
$myWindowsPrincipal=new-object System.Security.Principal.WindowsPrincipal($myWindowsID) 

# Get the security principal for the Administrator role
$adminRole=[System.Security.Principal.WindowsBuiltInRole]::Administrator 

# Check to see if we are currently running "as Administrator"
if ($myWindowsPrincipal.IsInRole($adminRole))
   {
   # We are running "as Administrator" - so change the title and background color to indicate this
   $Host.UI.RawUI.WindowTitle = $myInvocation.MyCommand.Definition + "(Elevated)"
   $Host.UI.RawUI.BackgroundColor = "DarkBlue"
   clear-host
   }
else
   {
   # We are not running "as Administrator" - so relaunch as administrator 

   # Create a new process object that starts PowerShell
   $newProcess = new-object System.Diagnostics.ProcessStartInfo "PowerShell";   

   # Specify the current script path and name as a parameter
   $newProcess.Arguments = $myInvocation.MyCommand.Definition;   

   # Indicate that the process should be elevated
   $newProcess.Verb = "runas";   

   # Start the new process
   [System.Diagnostics.Process]::Start($newProcess);   

   # Exit from the current, unelevated, process
   exit
   }

#################################################
# We are now elevated, do the actual registration
#################################################

# allow any user to bind to the url
netsh http add urlacl url=$endpointUrl user=everyone

# add an entry in hosts that maps it to localhost
Add-Content -Encoding UTF8 C:\Windows\system32\drivers\etc\hosts "127.0.0.1 $host"
