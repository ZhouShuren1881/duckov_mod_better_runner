## VS Code 编译配置

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "command": "dotnet",
      "type": "shell",
      "args": [
        "build",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "group": "build",
      "presentation": {
        "reveal": "silent"
      },
      "problemMatcher": "$msCompile"
    },
    {
      "label": "deploy",
      "type": "shell",
      "command": "powershell",
      "args": [
        "-ExecutionPolicy",
        "Bypass",
        "-File",
        // this file will copy products to duckov mod directory
        "./deploy.ps1"
      ],
      "presentation": {
        "reveal": "always"
      },
      "dependsOn": [],
      "problemMatcher": []
    },
    {
      "label": "build and deploy",
      "dependsOn": [
        "build",
        "deploy"
      ],
      "dependsOrder": "sequence",
      "problemMatcher": [],
      "group": {
        "kind": "build",
        "isDefault": true
      }
    }
  ]
}
```
