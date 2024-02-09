using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AoCTools.Loggers;

namespace AoC2023.Structures
{
    public class ModuleManager
    {
        public ModuleManager()
        {
            _button = new Button();
            _allModules.Add(_button);
        }

        private readonly List<Module> _allModules = new List<Module>();
        private readonly Module _button;

        public void AddModule(string name, string type)
        {
            Module existing;
            switch (type)
            {
                case "%":
                    existing = _allModules.FirstOrDefault(m => m.Name == name);
                    if (existing == null)
                        _allModules.Add(new FlipFlop(name));
                    else if (existing is UntypedModule)
                        Replace(existing, new FlipFlop(name));
                    break;
                case "&":
                    existing = _allModules.FirstOrDefault(m => m.Name == name);
                    if (existing == null)
                        _allModules.Add(new Conjunction(name));
                    else if (existing is UntypedModule)
                        Replace(existing, new Conjunction(name));
                    break;
                case "broadcaster":
                    existing = _allModules.FirstOrDefault(m => m.Name == name);
                    if (existing == null)
                    {
                        existing = new Broadcaster();
                        _allModules.Add(existing);
                    }
                    else if (existing is UntypedModule)
                    {
                        var bc = new Broadcaster();
                        Replace(existing, bc);
                        existing = bc;
                    }
                    Plug(_button, existing);
                    break;
                default:
                    existing = _allModules.FirstOrDefault(m => m.Name == name);
                    if (existing == null)
                        _allModules.Add(new UntypedModule(name));
                    break;
            }
        }

        private void Replace(Module remove, Module add)
        {
            add.Inputs.AddRange(remove.Inputs);
            foreach (var input in add.Inputs)
            {
                input.Outputs.Remove(remove);
                input.Outputs.Add(add);
            }

            add.Outputs.AddRange(remove.Outputs);
            foreach (var output in add.Outputs)
            {
                output.Inputs.Remove(remove);
                output.Inputs.Add(add);
            }

            _allModules.Remove(remove);
            _allModules.Add(add);
        }

        public void Plug(string moduleFrom, string moduleTo)
        {
            var from = _allModules.FirstOrDefault(m => m.Name == moduleFrom);
            if (from == null)
            {
                AddModule(moduleFrom, "?");
                from = _allModules.First(m => m.Name == moduleFrom);
            }
            var to = _allModules.FirstOrDefault(m => m.Name == moduleTo);
            if (to == null)
            {
                AddModule(moduleTo, "?");
                to = _allModules.First(m => m.Name == moduleTo);
            }

            Plug(from, to);
        }

        private void Plug(Module from, Module to)
        {
            from.Outputs.Add(to);
            to.Inputs.Add(from);
        }

        public void Pulse(bool log, out long lowPulses, out long highPulses, out bool rxMoved)
        {
            lowPulses = 0L;
            highPulses = 0L;
            rxMoved = false;
            var pulses = new List<PulseStep>
            {
                new PulseStep
                {

                    Sender = null,
                    IsHigh = false,
                    Receiver = _button
                }
            };

            while (pulses.Any())
            {
                var pulse = pulses[0];
                pulses.RemoveAt(0);

                if (log)
                    Logger.Log($"Pulsing {(pulse.IsHigh ? "HIGH" : "LOW")} from {(pulse.Sender == null ? "<elf>" : pulse.Sender.ToShortString())} to {pulse.Receiver.ToShortString()}");

                if (pulse.Receiver.Name == "rx" && pulse.IsHigh == false)
                {
                    Logger.Log($"RX MOVED!");
                    rxMoved = true;
                }

                if (pulse.Sender != null)
                {
                    if (pulse.IsHigh)
                        highPulses++;
                    else lowPulses++;
                }

                pulses.AddRange(pulse.Receiver.Pulse(pulse));
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== MODULE MANAGER ===");
            foreach (var module in _allModules)
                sb.AppendLine(module.ToString());
            return sb.ToString();
        }
    }

    public abstract class Module
    {
        public Module(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public List<Module> Inputs { get; } = new List<Module>();
        public List<Module> Outputs { get; } = new List<Module>();

        public string ToShortString()
        {
            return $"[{GetType().Name}] {Name}";
        }

        public override string ToString()
        {
            return $"[{GetType().Name}] {Name} -> {string.Join(" || ", Outputs.Select(o => o.ToShortString()))}";
        }

        /// <summary>
        /// Return output pulses, do not propagate by itself.
        /// </summary>
        public virtual IEnumerable<PulseStep> Pulse(PulseStep pulse)
        {
            throw new NotImplementedException();
        }
    }

    public class UntypedModule : Module
    {
        public UntypedModule(string name) : base(name)
        {
        }

        public override IEnumerable<PulseStep> Pulse(PulseStep pulse)
        {
            // does nothing
            return Array.Empty<PulseStep>();
        }
    }

    public class Button : Module
    {
        public Button() : base("button")
        {
        }

        public override IEnumerable<PulseStep> Pulse(PulseStep pulse)
        {
            var pulses = new List<PulseStep>();
            foreach (var output in Outputs)
            {
                pulses.Add(new PulseStep
                {
                    Sender = this,
                    IsHigh = false,
                    Receiver = output
                });
            }
            return pulses;
        }
    }

    public class Broadcaster : Module
    {
        public Broadcaster() : base("broadcaster")
        {
        }

        public override IEnumerable<PulseStep> Pulse(PulseStep pulse)
        {
            var pulses = new List<PulseStep>();
            foreach (var output in Outputs)
            {
                pulses.Add(new PulseStep
                {
                    Sender = this,
                    IsHigh = pulse.IsHigh,
                    Receiver = output
                });
            }
            return pulses;
        }
    }

    public class FlipFlop : Module
    {
        public FlipFlop(string name) : base(name)
        {
        }

        private bool _state = false;

        public override IEnumerable<PulseStep> Pulse(PulseStep pulse)
        {
            if (pulse.IsHigh)
                return Array.Empty<PulseStep>();

            _state = !_state;

            var pulses = new List<PulseStep>();
            foreach (var output in Outputs)
            {
                pulses.Add(new PulseStep
                {
                    Sender = this,
                    IsHigh = _state,
                    Receiver = output
                });
            }
            return pulses;
        }
    }

    public class Conjunction : Module
    {
        public Conjunction(string name) : base(name)
        {
        }

        private List<bool> _inputStates = null;

        public override IEnumerable<PulseStep> Pulse(PulseStep pulse)
        {
            if (_inputStates == null)
            {
                _inputStates = new List<bool>();
                foreach (var input in Inputs)
                    _inputStates.Add(false);
            }

            var senderIndex = Inputs.IndexOf(pulse.Sender);
            _inputStates[senderIndex] = pulse.IsHigh;

            var signal = _inputStates.Any(s => s == false);

            var pulses = new List<PulseStep>();
            foreach (var output in Outputs)
            {
                pulses.Add(new PulseStep
                {
                    Sender = this,
                    IsHigh = signal,
                    Receiver = output
                });
            }
            return pulses;
        }
    }

    public class PulseStep
    {
        public Module Sender { get; set; }
        public bool IsHigh { get; set; }
        public Module Receiver { get; set; }
    }
}