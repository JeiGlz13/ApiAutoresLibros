﻿using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAutores.Utilidades
{
    public class SwaggerAgrupaPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceController = controller.ControllerType.Namespace;
            var versionAPI = namespaceController.Split('.').Last().ToLower();
            controller.ApiExplorer.GroupName = versionAPI;
        }
    }
}
