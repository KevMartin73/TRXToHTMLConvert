# TRXToHRMLConvert
TRXToHRMLConvert is the command line tool for converting the output result of VSTest TRX log files to a more readable HTML format.
More details to follow.

# To run this tool from within visual studio
The following need to be set to valid locations.

    if (args.Length == 0)
    {
        args = new string[6];
        args[0] = "-i";
        args[1] = "C:\\Files\\testResults.trx";
        args[2] = "-o";
        args[3] = "C:\\Files\\testResults.html";
        args[4] = "-t";
        args[5] = "";
    }

testResults.trx must exist for the tool to complete.
testResults.html does not have to exist and if it does then the existing file will be replaced.
args[5] is optional, if you do not provide a template then the default will be used (template.cshtml).

# To run this tool from the command line
Open Command prompt and navigate to the folder that contains TRXToHTMLConvert.exe
Then type in 

    TRXToHTMLConvert -i C:\\TRXToHTML\\testResults.trx -o C:\\TRXToHTML\\testResults.html -t C:\\TRXToHTML\\template.cshtml

In this example -i is where the trx file is located
and -t is where the template is located
finally -o is where the html file will be created

# Running this fab tool within an azure pipeline
You will need to edit your YAML file either within Azure DevOps or using Notepad++ etc
First add these line before the tests are executed, the second line is optional, this will install the convert tool

    - script: dotnet tool install -g TRXToHTMLConvert
    displayName: 'Install TRXToHTMLConvert tool'

For my own YAML files my usual run order is 
    
    - task: VSBuild@1
        etc etc
    - script: pwsh D:\a\1\s\PelicanPiRegression\bin\Release\net9.0\playwright.ps1 install chromium --with-deps --only-shell
    - script: dotnet tool install -g TRXToHTMLConvert
    - task: VSTest@3

Then after the tests add these, this will create the actual HTML file from the TRX

    - script: TRXToHTMLConvert -i D:\a\_temp\TestResults\testResults.trx -o D:\a\_temp\TestResults\${{ section.module }}-testResults.html
      displayName: 'Convert TRX to HTML'

Finally add these, this will publish the HTML file as an artifact of the test run

      - task: PublishBuildArtifacts@1
        inputs:
          PathtoPublish: 'D:\a\_temp\TestResults\${{ section.module }}-testResults.html'
          ArtifactName: '${{ section.module }}-TestResults'
        displayName: 'Publish HTML'

Enjoy