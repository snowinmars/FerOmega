key=""
version="1.0.2.0"

mkdir -p _output
rm -rf _output
dotnet build -c Release /p:AssemblyVersion=$version

packages=$(ls _output | grep nupkg)

for package in $packages
do
  echo
  dotnet nuget push _output/$package -s https://api.nuget.org/v3/index.json -k $key
  echo
done
