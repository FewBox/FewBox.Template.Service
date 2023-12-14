echo 'Build FewBox.Template.Service...'
DOCKER_REPO_SLUG=fewbox/comy PROJECTNAME=FewBox.Template.Service PROJECTUNITTESTNAME=$PROJECTNAME.UnitTest
DOCKER_REPO_VERSION=v1
DOCKER_REPO_SERVER=registry.fewbox.lan
DOCKER_REPO_PORT=5000
dotnet restore $PROJECTNAME
dotnet publish -c Release $PROJECTNAME/$PROJECTNAME.csproj
cp Dockerfile ./$PROJECTNAME/bin/Release/net8.0/publish/Dockerfile
cp .dockerignore ./$PROJECTNAME/bin/Release/net8.0/publish/.dockerignore
cd $PROJECTNAME/bin/Release/net8.0/publish
docker build --no-cache -t $DOCKER_REPO_SERVER:$DOCKER_REPO_PORT/$DOCKER_REPO_SLUG:$DOCKER_REPO_VERSION .
docker push $DOCKER_REPO_SERVER:$DOCKER_REPO_PORT/$DOCKER_REPO_SLUG:$DOCKER_REPO_VERSION
cd ../../../../../
echo 'Finished!'