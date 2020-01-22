﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using Microsoft.Build.Unity.ProjectGeneration.Templates;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Build.Unity.ProjectGeneration.Exporters.TemplatedExporter
{
    /// <summary>
    /// A class for exporting platform props using templates.
    /// </summary>
    internal class TemplatedTopLevelDependenciesProjectExporter : ITopLevelDependenciesProjectExporter
    {
        private const string ProjectGuidToken = "PROJECT_GUID";

        private const string ProjectReferenceTemplate = "PROJECT_REFERENCE";
        private const string ProjectReferenceTemplate_ReferenceToken = "REFERENCE";
        private const string ProjectReferenceTemplate_ConditionToken = "CONDITION";

        private readonly FileTemplate primaryTemplateFile;
        private readonly FileTemplate propsTemplateFile;
        private readonly FileTemplate targetsTemplateFile;

        private readonly FileInfo primaryExportPath;
        private readonly FileInfo propsExportPath;
        private readonly FileInfo targetsExportPath;

        public Guid Guid { get; set; }

        public HashSet<ProjectReference> References { get; } = new HashSet<ProjectReference>();

        public TemplatedTopLevelDependenciesProjectExporter(FileTemplate primaryTemplateFile, FileTemplate propsTemplateFile, FileTemplate targetsTemplateFile,
            FileInfo primaryExportPath, FileInfo propsExportPath, FileInfo targetsExportPath)
        {
            this.primaryTemplateFile = primaryTemplateFile;
            this.propsTemplateFile = propsTemplateFile;
            this.targetsTemplateFile = targetsTemplateFile;

            this.primaryExportPath = primaryExportPath;
            this.propsExportPath = propsExportPath;
            this.targetsExportPath = targetsExportPath;
        }

        public void Write()
        {
            TemplatedWriter propsWriter = new TemplatedWriter(propsTemplateFile);

            propsWriter.Write(ProjectGuidToken, Guid.ToString().ToUpper());

            foreach (ProjectReference projectReference in References)
            {
                TemplatedWriter referenceWriter = propsWriter.CreateWriterFor(ProjectReferenceTemplate);
                referenceWriter.Write(ProjectReferenceTemplate_ReferenceToken, projectReference.ReferencePath.LocalPath);
                referenceWriter.Write(ProjectReferenceTemplate_ConditionToken, projectReference.Condition ?? string.Empty);
            }

            propsWriter.Export(propsExportPath);

            new TemplatedWriter(primaryTemplateFile).Export(primaryExportPath);
            new TemplatedWriter(targetsTemplateFile).Export(targetsExportPath);
        }
    }
}
#endif