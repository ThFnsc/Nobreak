﻿using AutoMapper;
using Nobreak.Context.Entities;
using Nobreak.Infra.Context.Entities;
using Nobreak.Infra.Services.Report;
using Nobreak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nobreak
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<NobreakState, NobreakStateViewModel>();
            CreateMap<NobreakStateChange, NobreakStateChangeViewModel>();
            CreateMap<UptimeState, UptimeStateViewModel>();
            CreateMap<UptimeInInterval, UptimeInIntervalViewModel>();
            CreateMap<UptimeReport, UptimeReportViewModel>();
        }
    }
}
